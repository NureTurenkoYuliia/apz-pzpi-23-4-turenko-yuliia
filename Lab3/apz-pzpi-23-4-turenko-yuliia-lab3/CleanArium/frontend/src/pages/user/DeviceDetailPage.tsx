import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import {
  ArrowLeft,
  RefreshCw,
  Plus,
  Pencil,
  Trash2,
  BarChart2,
} from "lucide-react";
import toast from "react-hot-toast";
import { deviceApi } from "../../api/device";
import { alarmRuleApi } from "../../api/alarmRule";
import { scheduledCommandApi } from "../../api/scheduledCommand";
import {
  AlarmRuleDto,
  CommandType,
  ConditionType,
  RepeatMode,
  ScheduledCommandDto,
  SensorDataDto,
} from "../../types";
import LoadingSpinner from "../../components/common/LoadingSpinner";
import ConfirmModal from "../../components/modals/ConfirmModal";
import FormModal from "../../components/common/FormModal";
import StatusBadge from "../../components/common/StatusBadge";
import FileImportButton from "../../components/common/FileImportButton";
import AlarmRuleAnalysisModal from "../../components/modals/AlarmRuleAnalysisModal";

const alarmSchema = (t: (k: string) => string) =>
  z.object({
    condition: z.coerce.number().int().min(1),
    threshold: z.coerce.number(),
    unit: z.string().min(1, t("validation.required")).max(20),
    isActive: z.boolean().optional(),
  });
type AlarmFormValues = {
  condition: number;
  threshold: number;
  unit: string;
  isActive?: boolean;
};

const schedSchema = (t: (k: string) => string) =>
  z.object({
    commandType: z.coerce.number().int().min(1),
    startTime: z.string().min(1, t("validation.required")),
    repeatMode: z.coerce.number().int().min(1),
    intervalMinutes: z.coerce.number().int().min(1).nullable().optional(),
    isActive: z.boolean().optional(),
  });
type SchedFormValues = {
  commandType: number;
  startTime: string;
  repeatMode: number;
  intervalMinutes?: number | null;
  isActive?: boolean;
};

const cmdSchema = z.object({ commandType: z.coerce.number().int().min(1) });
type CmdFormValues = { commandType: number };

const SectionHeader = ({
  title,
  onAdd,
  importButton,
}: {
  title: string;
  onAdd: () => void;
  importButton?: React.ReactNode;
}) => (
  <div className="flex items-center justify-between mb-3">
    <h2 className="font-display text-xl text-primary">{title}</h2>
    <div className="flex items-center gap-2">
      {importButton}
      <button
        onClick={onAdd}
        className="inline-flex items-center gap-1 px-3 py-1.5 bg-primary text-white text-sm font-medium rounded-xl hover:bg-primary-light transition-colors"
      >
        <Plus className="w-3.5 h-3.5" />
      </button>
    </div>
  </div>
);

const DeviceDetailPage = () => {
  const { t } = useTranslation();
  const { aquariumId, deviceId } = useParams<{
    aquariumId: string;
    deviceId: string;
  }>();
  const aqId = Number(aquariumId);
  const devId = Number(deviceId);

  const [sensor, setSensor] = useState<SensorDataDto | null>(null);
  const [sensorLoading, setSensorLoading] = useState(false);

  const [rules, setRules] = useState<AlarmRuleDto[]>([]);
  const [rulesLoading, setRulesLoading] = useState(true);
  const [alarmModal, setAlarmModal] = useState(false);
  const [editingRule, setEditingRule] = useState<AlarmRuleDto | null>(null);
  const [deleteRuleId, setDeleteRuleId] = useState<number | null>(null);
  const [deletingRule, setDeletingRule] = useState(false);
  const [analysisRuleId, setAnalysisRuleId] = useState<number | null>(null);

  const [commands, setCommands] = useState<ScheduledCommandDto[]>([]);
  const [cmdsLoading, setCmdsLoading] = useState(true);
  const [schedModal, setSchedModal] = useState(false);
  const [editingCmd, setEditingCmd] = useState<ScheduledCommandDto | null>(
    null,
  );
  const [deleteCmdId, setDeleteCmdId] = useState<number | null>(null);
  const [deletingCmd, setDeletingCmd] = useState(false);

  const [execModal, setExecModal] = useState(false);
  const [executing, setExecuting] = useState(false);

  const [importingRules, setImportingRules] = useState(false);
  const [importingCmds, setImportingCmds] = useState(false);

  const alarmForm = useForm<AlarmFormValues>({
    resolver: zodResolver(alarmSchema(t)),
    defaultValues: {
      condition: ConditionType.Greater,
      threshold: 0,
      unit: "",
      isActive: true,
    },
  });
  const schedForm = useForm<SchedFormValues>({
    resolver: zodResolver(schedSchema(t)),
    defaultValues: {
      commandType: CommandType.TurnOn,
      startTime: "",
      repeatMode: RepeatMode.None,
      intervalMinutes: null,
      isActive: true,
    },
  });
  const cmdForm = useForm<CmdFormValues>({
    resolver: zodResolver(cmdSchema),
    defaultValues: { commandType: CommandType.TurnOn },
  });

  const watchRepeat = schedForm.watch("repeatMode");

  useEffect(() => {
    loadSensor();
    loadRules();
    loadCommands();
  }, [devId]);

  const loadSensor = async () => {
    setSensorLoading(true);
    try {
      setSensor(await deviceApi.getSensorData(devId));
    } catch {
      setSensor(null);
    } finally {
      setSensorLoading(false);
    }
  };

  const loadRules = async () => {
    setRulesLoading(true);
    try {
      setRules(await alarmRuleApi.getByDevice(devId));
    } catch {
      toast.error(t("alarmRule.loadError"));
    } finally {
      setRulesLoading(false);
    }
  };

  const loadCommands = async () => {
    setCmdsLoading(true);
    try {
      setCommands(await scheduledCommandApi.getByDevice(devId));
    } catch {
      toast.error(t("scheduledCommand.loadError"));
    } finally {
      setCmdsLoading(false);
    }
  };

  const openCreateRule = () => {
    setEditingRule(null);
    alarmForm.reset({
      condition: ConditionType.Greater,
      threshold: 0,
      unit: "",
      isActive: true,
    });
    setAlarmModal(true);
  };
  const openEditRule = (r: AlarmRuleDto) => {
    setEditingRule(r);
    alarmForm.reset({
      condition: r.condition,
      threshold: r.threshold,
      unit: r.unit,
      isActive: r.isActive,
    });
    setAlarmModal(true);
  };

  const onAlarmSubmit = async (values: AlarmFormValues) => {
    try {
      if (editingRule) {
        await alarmRuleApi.update({
          ruleId: editingRule.id,
          condition: values.condition,
          threshold: values.threshold,
          unit: values.unit,
          isActive: values.isActive ?? true,
        });
        toast.success(t("alarmRule.updateSuccess"));
        setRules((prev) =>
          prev.map((r) =>
            r.id === editingRule.id
              ? { ...r, ...values, isActive: values.isActive ?? true }
              : r,
          ),
        );
      } else {
        const id = await alarmRuleApi.create({
          deviceId: devId,
          condition: values.condition,
          threshold: values.threshold,
          unit: values.unit,
        });
        toast.success(t("alarmRule.createSuccess"));
        setRules((prev) => [
          ...prev,
          {
            id,
            deviceId: devId,
            condition: values.condition,
            threshold: values.threshold,
            unit: values.unit,
            isActive: true,
          },
        ]);
      }
      setAlarmModal(false);
    } catch {
      toast.error(t("common.error"));
    }
  };

  const handleDeleteRule = async () => {
    if (!deleteRuleId) return;
    setDeletingRule(true);
    try {
      await alarmRuleApi.delete(deleteRuleId);
      setRules((prev) => prev.filter((r) => r.id !== deleteRuleId));
      toast.success(t("alarmRule.deleteSuccess"));
    } catch {
      toast.error(t("common.error"));
    } finally {
      setDeletingRule(false);
      setDeleteRuleId(null);
    }
  };

  const handleImportRules = async (file: File) => {
    setImportingRules(true);
    try {
      await alarmRuleApi.import(devId, file);
      toast.success(t("alarmRule.importSuccess"));
      await loadRules();
    } catch {
      toast.error(t("common.error"));
    } finally {
      setImportingRules(false);
    }
  };

  const openCreateCmd = () => {
    setEditingCmd(null);
    schedForm.reset({
      commandType: CommandType.TurnOn,
      startTime: "",
      repeatMode: RepeatMode.None,
      intervalMinutes: null,
      isActive: true,
    });
    setSchedModal(true);
  };
  const openEditCmd = (c: ScheduledCommandDto) => {
    setEditingCmd(c);
    schedForm.reset({
      commandType: c.commandType,
      startTime: c.startTime.slice(0, 16),
      repeatMode: c.repeatMode,
      intervalMinutes: c.intervalMinutes,
      isActive: c.isActive,
    });
    setSchedModal(true);
  };

  const onSchedSubmit = async (values: SchedFormValues) => {
    const intervalMinutes =
      Number(values.repeatMode) === RepeatMode.Interval
        ? (values.intervalMinutes ?? null)
        : null;
    try {
      if (editingCmd) {
        await scheduledCommandApi.update({
          id: editingCmd.id,
          commandType: values.commandType,
          startTime: values.startTime,
          repeatMode: values.repeatMode,
          intervalMinutes,
          isActive: values.isActive ?? true,
        });
        toast.success(t("scheduledCommand.updateSuccess"));
        setCommands((prev) =>
          prev.map((c) =>
            c.id === editingCmd.id
              ? {
                  ...c,
                  ...values,
                  intervalMinutes,
                  isActive: values.isActive ?? true,
                }
              : c,
          ),
        );
      } else {
        const id = await scheduledCommandApi.create({
          deviceId: devId,
          commandType: values.commandType,
          startTime: values.startTime,
          repeatMode: values.repeatMode,
          intervalMinutes,
          isActive: true,
        });
        toast.success(t("scheduledCommand.createSuccess"));
        setCommands((prev) => [
          ...prev,
          {
            id,
            deviceId: devId,
            commandType: values.commandType,
            startTime: values.startTime,
            repeatMode: values.repeatMode,
            intervalMinutes,
            isActive: true,
          },
        ]);
      }
      setSchedModal(false);
    } catch {
      toast.error(t("common.error"));
    }
  };

  const handleDeleteCmd = async () => {
    if (!deleteCmdId) return;
    setDeletingCmd(true);
    try {
      await scheduledCommandApi.delete(deleteCmdId);
      setCommands((prev) => prev.filter((c) => c.id !== deleteCmdId));
      toast.success(t("scheduledCommand.deleteSuccess"));
    } catch {
      toast.error(t("common.error"));
    } finally {
      setDeletingCmd(false);
      setDeleteCmdId(null);
    }
  };

  const handleImportCmds = async (file: File) => {
    setImportingCmds(true);
    try {
      await scheduledCommandApi.import(devId, file);
      toast.success(t("scheduledCommand.importSuccess"));
      await loadCommands();
    } catch {
      toast.error(t("common.error"));
    } finally {
      setImportingCmds(false);
    }
  };

  const onExecute = async (values: CmdFormValues) => {
    setExecuting(true);
    try {
      await deviceApi.executeCommand(devId, {
        commandType: values.commandType,
        commandStatus: 3,
      });
      toast.success(t("device.commandSuccess"));
      setExecModal(false);
    } catch {
      toast.error(t("device.commandSendError"));
    } finally {
      setExecuting(false);
    }
  };

  const cmdTypeOptions = [
    [CommandType.TurnOn, t("analytics.commandTypes.1")],
    [CommandType.TurnOff, t("analytics.commandTypes.2")],
    [CommandType.SetValue, t("analytics.commandTypes.3")],
    [CommandType.Calibrate, t("analytics.commandTypes.4")],
  ] as [number, string][];

  const conditionOptions = Object.entries({
    1: t("alarmRule.conditions.1"),
    2: t("alarmRule.conditions.2"),
    3: t("alarmRule.conditions.3"),
    4: t("alarmRule.conditions.4"),
    5: t("alarmRule.conditions.5"),
    6: t("alarmRule.conditions.6"),
  });

  const repeatModeOptions = Object.entries({
    1: t("scheduledCommand.repeatModes.1"),
    2: t("scheduledCommand.repeatModes.2"),
    3: t("scheduledCommand.repeatModes.3"),
    4: t("scheduledCommand.repeatModes.4"),
  });

  return (
    <div className="space-y-8">
      <Link
        to={`/user/aquariums/${aqId}`}
        className="inline-flex items-center gap-1 text-sm text-primary/50 hover:text-primary transition-colors"
      >
        <ArrowLeft className="w-3.5 h-3.5" /> {t("device.backToDevices")}
      </Link>

      <div className="bg-white rounded-2xl shadow-card p-6">
        <div className="flex items-center justify-between mb-4">
          <h2 className="font-display text-xl text-primary">
            {t("device.sensorData")}
          </h2>
          <div className="flex items-center gap-2">
            <button
              onClick={loadSensor}
              disabled={sensorLoading}
              className="p-1.5 rounded-lg text-primary/40 hover:text-primary hover:bg-background transition-colors disabled:opacity-40"
            >
              <RefreshCw
                className={`w-4 h-4 ${sensorLoading ? "animate-spin" : ""}`}
              />
            </button>
            <button
              onClick={() => setExecModal(true)}
              className="px-3 py-1.5 bg-primary text-white text-sm font-medium rounded-xl hover:bg-primary-light transition-colors"
            >
              {t("device.executeCommand")}
            </button>
          </div>
        </div>
        {sensorLoading ? (
          <LoadingSpinner size="sm" />
        ) : sensor ? (
          <div className="flex gap-6">
            <div>
              <p className="text-xs text-primary/40 font-body uppercase tracking-wide mb-0.5">
                {t("device.sensorData")}
              </p>
              <p className="font-display text-3xl text-primary">
                {sensor.value}{" "}
                <span className="text-lg text-primary/60">{sensor.unit}</span>
              </p>
            </div>
            <div className="border-l border-background-muted pl-6">
              <p className="text-xs text-primary/40 font-body uppercase tracking-wide mb-0.5">
                {t("device.sensorData")}
              </p>
              <p className="text-sm font-body text-primary/70">
                {new Date(sensor.dateTime).toLocaleString()}
              </p>
            </div>
          </div>
        ) : (
          <p className="text-sm text-primary/40 font-body">
            {t("device.noSensor")}
          </p>
        )}
      </div>

      {/* Alarm rules */}
      <div className="bg-white rounded-2xl shadow-card p-6">
        <SectionHeader
          title={t("alarmRule.title")}
          onAdd={openCreateRule}
          importButton={
            <FileImportButton
              onFile={handleImportRules}
              accept=".json,.csv"
              loading={importingRules}
            />
          }
        />
        {rulesLoading ? (
          <LoadingSpinner />
        ) : rules.length === 0 ? (
          <p className="text-sm text-primary/40 font-body text-center py-6">
            {t("alarmRule.empty")}
          </p>
        ) : (
          <div className="space-y-2">
            {rules.map((r) => (
              <div
                key={r.id}
                className="flex items-center justify-between p-3 rounded-xl border border-background-muted hover:bg-background/50 transition-colors"
              >
                <div className="flex items-center gap-3">
                  <StatusBadge
                    active={r.isActive}
                    activeLabel={t("common.active")}
                    inactiveLabel={t("common.inactive")}
                  />
                  <span className="text-sm font-body text-primary">
                    {t(`alarmRule.conditions.${r.condition}`)}{" "}
                    <strong>{r.threshold}</strong> {r.unit}
                  </span>
                </div>
                <div className="flex items-center gap-1">
                  <button
                    onClick={() => setAnalysisRuleId(r.id)}
                    className="p-1.5 rounded-lg text-primary/40 hover:text-primary hover:bg-background transition-colors"
                    title={t("common.analyze")}
                  >
                    <BarChart2 className="w-4 h-4" />
                  </button>
                  <button
                    onClick={() => openEditRule(r)}
                    className="p-1.5 rounded-lg text-primary/40 hover:text-primary hover:bg-background transition-colors"
                  >
                    <Pencil className="w-4 h-4" />
                  </button>
                  <button
                    onClick={() => setDeleteRuleId(r.id)}
                    className="p-1.5 rounded-lg text-primary/40 hover:text-red-500 hover:bg-red-50 transition-colors"
                  >
                    <Trash2 className="w-4 h-4" />
                  </button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      <div className="bg-white rounded-2xl shadow-card p-6">
        <SectionHeader
          title={t("scheduledCommand.title")}
          onAdd={openCreateCmd}
          importButton={
            <FileImportButton
              onFile={handleImportCmds}
              accept=".json,.csv"
              loading={importingCmds}
            />
          }
        />
        {cmdsLoading ? (
          <LoadingSpinner />
        ) : commands.length === 0 ? (
          <p className="text-sm text-primary/40 font-body text-center py-6">
            {t("scheduledCommand.empty")}
          </p>
        ) : (
          <div className="space-y-2">
            {commands.map((c) => (
              <div
                key={c.id}
                className="flex items-center justify-between p-3 rounded-xl border border-background-muted hover:bg-background/50 transition-colors"
              >
                <div className="flex items-center gap-3">
                  <StatusBadge
                    active={c.isActive}
                    activeLabel={t("common.active")}
                    inactiveLabel={t("common.inactive")}
                  />
                  <div>
                    <p className="text-sm font-body font-medium text-primary">
                      {t(`analytics.commandTypes.${c.commandType}`)}
                    </p>
                    <p className="text-xs font-body text-primary/50">
                      {new Date(c.startTime).toLocaleString()} ·{" "}
                      {t(`scheduledCommand.repeatModes.${c.repeatMode}`)}
                      {c.intervalMinutes
                        ? ` (${c.intervalMinutes} ${t("scheduledCommand.intervalMinutes")})`
                        : ""}
                    </p>
                  </div>
                </div>
                <div className="flex items-center gap-1">
                  <button
                    onClick={() => openEditCmd(c)}
                    className="p-1.5 rounded-lg text-primary/40 hover:text-primary hover:bg-background transition-colors"
                  >
                    <Pencil className="w-4 h-4" />
                  </button>
                  <button
                    onClick={() => setDeleteCmdId(c.id)}
                    className="p-1.5 rounded-lg text-primary/40 hover:text-red-500 hover:bg-red-50 transition-colors"
                  >
                    <Trash2 className="w-4 h-4" />
                  </button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      <FormModal
        isOpen={execModal}
        title={t("device.executeCommand")}
        onClose={() => setExecModal(false)}
      >
        <form onSubmit={cmdForm.handleSubmit(onExecute)} className="space-y-4">
          <div>
            <label className="block text-sm font-body font-medium text-primary/70 mb-1.5">
              {t("device.commandType")}
            </label>
            <select
              {...cmdForm.register("commandType")}
              className="w-full border border-secondary/40 rounded-xl px-4 py-2.5 text-sm font-body text-primary focus:outline-none focus:border-primary bg-white"
            >
              {cmdTypeOptions.map(([v, label]) => (
                <option key={v} value={v}>
                  {label}
                </option>
              ))}
            </select>
          </div>
          <div className="flex justify-end gap-3 pt-2">
            <button
              type="button"
              onClick={() => setExecModal(false)}
              className="px-4 py-2 text-sm text-primary/60 hover:text-primary transition-colors"
            >
              {t("common.cancel")}
            </button>
            <button
              type="submit"
              disabled={executing}
              className="px-5 py-2 bg-primary text-white text-sm font-medium rounded-xl hover:bg-primary-light transition-colors disabled:opacity-60"
            >
              {executing ? t("common.loading") : t("device.executeCommand")}
            </button>
          </div>
        </form>
      </FormModal>

      <FormModal
        isOpen={alarmModal}
        title={t(editingRule ? "alarmRule.editTitle" : "alarmRule.createTitle")}
        onClose={() => setAlarmModal(false)}
      >
        <form
          onSubmit={alarmForm.handleSubmit(onAlarmSubmit)}
          className="space-y-4"
        >
          <div>
            <label className="block text-sm font-body font-medium text-primary/70 mb-1.5">
              {t("alarmRule.condition")}
            </label>
            <select
              {...alarmForm.register("condition")}
              className="w-full border border-secondary/40 rounded-xl px-4 py-2.5 text-sm font-body text-primary focus:outline-none focus:border-primary bg-white"
            >
              {conditionOptions.map(([v, label]) => (
                <option key={v} value={v}>
                  {label}
                </option>
              ))}
            </select>
          </div>
          <div>
            <label className="block text-sm font-body font-medium text-primary/70 mb-1.5">
              {t("alarmRule.threshold")}
            </label>
            <input
              type="number"
              step="any"
              {...alarmForm.register("threshold")}
              className="w-full border border-secondary/40 rounded-xl px-4 py-2.5 text-sm font-body text-primary focus:outline-none focus:border-primary transition-colors"
            />
            {alarmForm.formState.errors.threshold && (
              <p className="text-red-500 text-xs mt-1">
                {alarmForm.formState.errors.threshold.message}
              </p>
            )}
          </div>
          <div>
            <label className="block text-sm font-body font-medium text-primary/70 mb-1.5">
              {t("alarmRule.unit")}
            </label>
            <input
              {...alarmForm.register("unit")}
              placeholder={t("alarmRule.unitPlaceholder")}
              className="w-full border border-secondary/40 rounded-xl px-4 py-2.5 text-sm font-body text-primary focus:outline-none focus:border-primary transition-colors"
            />
            {alarmForm.formState.errors.unit && (
              <p className="text-red-500 text-xs mt-1">
                {alarmForm.formState.errors.unit.message}
              </p>
            )}
          </div>
          {editingRule && (
            <label className="flex items-center gap-2 text-sm font-body text-primary/70 cursor-pointer">
              <input
                type="checkbox"
                {...alarmForm.register("isActive")}
                className="rounded"
              />
              {t("alarmRule.isActive")}
            </label>
          )}
          <div className="flex justify-end gap-3 pt-2">
            <button
              type="button"
              onClick={() => setAlarmModal(false)}
              className="px-4 py-2 text-sm text-primary/60 hover:text-primary transition-colors"
            >
              {t("common.cancel")}
            </button>
            <button
              type="submit"
              className="px-5 py-2 bg-primary text-white text-sm font-medium rounded-xl hover:bg-primary-light transition-colors"
            >
              {t("common.save")}
            </button>
          </div>
        </form>
      </FormModal>

      <FormModal
        isOpen={schedModal}
        title={t(
          editingCmd
            ? "scheduledCommand.editTitle"
            : "scheduledCommand.createTitle",
        )}
        onClose={() => setSchedModal(false)}
      >
        <form
          onSubmit={schedForm.handleSubmit(onSchedSubmit)}
          className="space-y-4"
        >
          <div>
            <label className="block text-sm font-body font-medium text-primary/70 mb-1.5">
              {t("scheduledCommand.commandType")}
            </label>
            <select
              {...schedForm.register("commandType")}
              className="w-full border border-secondary/40 rounded-xl px-4 py-2.5 text-sm font-body text-primary focus:outline-none focus:border-primary bg-white"
            >
              {cmdTypeOptions.map(([v, label]) => (
                <option key={v} value={v}>
                  {label}
                </option>
              ))}
            </select>
          </div>
          <div>
            <label className="block text-sm font-body font-medium text-primary/70 mb-1.5">
              {t("scheduledCommand.startTime")}
            </label>
            <input
              type="datetime-local"
              {...schedForm.register("startTime")}
              className="w-full border border-secondary/40 rounded-xl px-4 py-2.5 text-sm font-body text-primary focus:outline-none focus:border-primary transition-colors"
            />
            {schedForm.formState.errors.startTime && (
              <p className="text-red-500 text-xs mt-1">
                {schedForm.formState.errors.startTime.message}
              </p>
            )}
          </div>
          <div>
            <label className="block text-sm font-body font-medium text-primary/70 mb-1.5">
              {t("scheduledCommand.repeatMode")}
            </label>
            <select
              {...schedForm.register("repeatMode")}
              className="w-full border border-secondary/40 rounded-xl px-4 py-2.5 text-sm font-body text-primary focus:outline-none focus:border-primary bg-white"
            >
              {repeatModeOptions.map(([v, label]) => (
                <option key={v} value={v}>
                  {label}
                </option>
              ))}
            </select>
          </div>
          {Number(watchRepeat) === RepeatMode.Interval && (
            <div>
              <label className="block text-sm font-body font-medium text-primary/70 mb-1.5">
                {t("scheduledCommand.intervalMinutes")}
              </label>
              <input
                type="number"
                min={1}
                {...schedForm.register("intervalMinutes")}
                className="w-full border border-secondary/40 rounded-xl px-4 py-2.5 text-sm font-body text-primary focus:outline-none focus:border-primary transition-colors"
              />
            </div>
          )}
          {editingCmd && (
            <label className="flex items-center gap-2 text-sm font-body text-primary/70 cursor-pointer">
              <input
                type="checkbox"
                {...schedForm.register("isActive")}
                className="rounded"
              />
              {t("scheduledCommand.isActive")}
            </label>
          )}
          <div className="flex justify-end gap-3 pt-2">
            <button
              type="button"
              onClick={() => setSchedModal(false)}
              className="px-4 py-2 text-sm text-primary/60 hover:text-primary transition-colors"
            >
              {t("common.cancel")}
            </button>
            <button
              type="submit"
              className="px-5 py-2 bg-primary text-white text-sm font-medium rounded-xl hover:bg-primary-light transition-colors"
            >
              {t("common.save")}
            </button>
          </div>
        </form>
      </FormModal>

      <ConfirmModal
        isOpen={!!deleteRuleId}
        message={t("alarmRule.confirmDelete")}
        onConfirm={handleDeleteRule}
        onCancel={() => setDeleteRuleId(null)}
        danger
        loading={deletingRule}
      />
      <ConfirmModal
        isOpen={!!deleteCmdId}
        message={t("scheduledCommand.confirmDelete")}
        onConfirm={handleDeleteCmd}
        onCancel={() => setDeleteCmdId(null)}
        danger
        loading={deletingCmd}
      />

      <AlarmRuleAnalysisModal
        ruleId={analysisRuleId}
        onClose={() => setAnalysisRuleId(null)}
      />
    </div>
  );
};

export default DeviceDetailPage;
