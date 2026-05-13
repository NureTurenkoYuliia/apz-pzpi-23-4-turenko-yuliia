import { useState } from "react";
import { useTranslation } from "react-i18next";
import { ShieldOff, BarChart2 } from "lucide-react";
import toast from "react-hot-toast";
import { alarmRuleApi } from "../../api/alarmRule";
import { scheduledCommandApi } from "../../api/scheduledCommand";
import { AlarmRuleDto, ScheduledCommandDto } from "../../types";
import LoadingSpinner from "../common/LoadingSpinner";
import ConfirmModal from "../modals/ConfirmModal";
import StatusBadge from "../common/StatusBadge";
import AlarmRuleAnalysisModal from "../modals/AlarmRuleAnalysisModal";

const DeviceRulesPanel = () => {
  const { t } = useTranslation();
  const [deviceId, setDeviceId] = useState("");

  const [rules, setRules] = useState<AlarmRuleDto[]>([]);
  const [rulesLoading, setRulesLoading] = useState(false);
  const [rulesFetched, setRulesFetched] = useState(false);

  const [commands, setCommands] = useState<ScheduledCommandDto[]>([]);
  const [cmdsLoading, setCmdsLoading] = useState(false);
  const [cmdsFetched, setCmdsFetched] = useState(false);

  const [deactivateRuleId, setDeactivateRuleId] = useState<number | null>(null);
  const [deactivatingRule, setDeactivatingRule] = useState(false);

  const [deactivateCmdId, setDeactivateCmdId] = useState<number | null>(null);
  const [deactivatingCmd, setDeactivatingCmd] = useState(false);

  const [analysisRuleId, setAnalysisRuleId] = useState<number | null>(null);

  const id = Number(deviceId);

  const loadRules = async () => {
    if (!id) return;
    setRulesLoading(true);
    try {
      setRules(await alarmRuleApi.getByDevice(id));
      setRulesFetched(true);
    } catch {
      toast.error(t("alarmRule.loadError"));
    } finally {
      setRulesLoading(false);
    }
  };

  const loadCommands = async () => {
    if (!id) return;
    setCmdsLoading(true);
    try {
      setCommands(await scheduledCommandApi.getByDevice(id));
      setCmdsFetched(true);
    } catch {
      toast.error(t("scheduledCommand.loadError"));
    } finally {
      setCmdsLoading(false);
    }
  };

  const handleDeactivateRule = async () => {
    if (!deactivateRuleId) return;
    setDeactivatingRule(true);
    try {
      await alarmRuleApi.deactivate(deactivateRuleId);
      setRules((prev) =>
        prev.map((r) =>
          r.id === deactivateRuleId ? { ...r, isActive: false } : r,
        ),
      );
      toast.success(t("analytics.deactivateRuleSuccess"));
    } catch {
      toast.error(t("common.error"));
    } finally {
      setDeactivatingRule(false);
      setDeactivateRuleId(null);
    }
  };

  const handleDeactivateCmd = async () => {
    if (!deactivateCmdId) return;
    setDeactivatingCmd(true);
    try {
      await scheduledCommandApi.deactivate(deactivateCmdId);
      setCommands((prev) =>
        prev.map((c) =>
          c.id === deactivateCmdId ? { ...c, isActive: false } : c,
        ),
      );
      toast.success(t("analytics.deactivateScheduledSuccess"));
    } catch {
      toast.error(t("common.error"));
    } finally {
      setDeactivatingCmd(false);
      setDeactivateCmdId(null);
    }
  };

  return (
    <div className="space-y-6">
      <div className="bg-white rounded-2xl shadow-card p-5">
        <div className="flex items-center gap-3">
          <label className="text-sm font-body text-primary/60 whitespace-nowrap">
            {t("analytics.deviceIdInput")}
          </label>
          <input
            type="number"
            min={1}
            value={deviceId}
            onChange={(e) => setDeviceId(e.target.value)}
            className="w-32 border border-secondary/40 rounded-lg px-3 py-1.5 text-sm font-body text-primary focus:outline-none focus:border-primary transition-colors"
            placeholder="1"
          />
          <button
            onClick={loadRules}
            disabled={!deviceId || rulesLoading}
            className="px-3 py-1.5 bg-primary text-white text-sm font-medium rounded-xl hover:bg-primary-light transition-colors disabled:opacity-50"
          >
            {t("analytics.loadRules")}
          </button>
          <button
            onClick={loadCommands}
            disabled={!deviceId || cmdsLoading}
            className="px-3 py-1.5 border border-secondary/40 text-primary text-sm font-medium rounded-xl hover:bg-background transition-colors disabled:opacity-50"
          >
            {t("analytics.loadScheduled")}
          </button>
        </div>
      </div>

      {(rulesLoading || rulesFetched) && (
        <div className="bg-white rounded-2xl shadow-card p-6">
          <h3 className="font-display text-lg text-primary mb-4">
            {t("analytics.rules")}
          </h3>
          {rulesLoading ? (
            <LoadingSpinner />
          ) : rules.length === 0 ? (
            <p className="text-sm text-primary/40 font-body text-center py-4">
              {t("common.noData")}
            </p>
          ) : (
            <div className="space-y-2">
              {rules.map((r) => (
                <div
                  key={r.id}
                  className="flex items-center justify-between p-3 rounded-xl border border-background-muted"
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
                    {r.isActive && (
                      <button
                        onClick={() => setDeactivateRuleId(r.id)}
                        className="inline-flex items-center gap-1 px-2.5 py-1 text-xs font-medium text-amber-600 border border-amber-200 rounded-lg hover:bg-amber-50 transition-colors"
                      >
                        <ShieldOff className="w-3 h-3" />{" "}
                        {t("common.deactivate")}
                      </button>
                    )}
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      )}

      {(cmdsLoading || cmdsFetched) && (
        <div className="bg-white rounded-2xl shadow-card p-6">
          <h3 className="font-display text-lg text-primary mb-4">
            {t("analytics.scheduled")}
          </h3>
          {cmdsLoading ? (
            <LoadingSpinner />
          ) : commands.length === 0 ? (
            <p className="text-sm text-primary/40 font-body text-center py-4">
              {t("common.noData")}
            </p>
          ) : (
            <div className="space-y-2">
              {commands.map((c) => (
                <div
                  key={c.id}
                  className="flex items-center justify-between p-3 rounded-xl border border-background-muted"
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
                      </p>
                    </div>
                  </div>
                  {c.isActive && (
                    <button
                      onClick={() => setDeactivateCmdId(c.id)}
                      className="inline-flex items-center gap-1 px-2.5 py-1 text-xs font-medium text-amber-600 border border-amber-200 rounded-lg hover:bg-amber-50 transition-colors"
                    >
                      <ShieldOff className="w-3 h-3" /> {t("common.deactivate")}
                    </button>
                  )}
                </div>
              ))}
            </div>
          )}
        </div>
      )}

      <ConfirmModal
        isOpen={!!deactivateRuleId}
        message={t("analytics.confirmDeactivateRule")}
        onConfirm={handleDeactivateRule}
        onCancel={() => setDeactivateRuleId(null)}
        loading={deactivatingRule}
      />
      <ConfirmModal
        isOpen={!!deactivateCmdId}
        message={t("analytics.confirmDeactivateScheduled")}
        onConfirm={handleDeactivateCmd}
        onCancel={() => setDeactivateCmdId(null)}
        loading={deactivatingCmd}
      />
      <AlarmRuleAnalysisModal
        ruleId={analysisRuleId}
        onClose={() => setAnalysisRuleId(null)}
      />
    </div>
  );
};

export default DeviceRulesPanel;
