import { useEffect, useRef, useState } from "react";
import { useTranslation } from "react-i18next";
import {
  RefreshCw,
  RotateCcw,
  Maximize2,
  Minus,
  Plus,
  Loader2,
} from "lucide-react";
import toast from "react-hot-toast";
import { kubernetesApi } from "../../api/kubernetes";
import { DeploymentDto, PodDto, PodLogsDto } from "../../types";
import LoadingSpinner from "../../components/common/LoadingSpinner";
import ConfirmModal from "../../components/modals/ConfirmModal";
import FormModal from "../../components/common/FormModal";
import KubernetesConfigGenerator from "../../components/charts/KubernetesConfigGenerator";

const statusColors: Record<string, string> = {
  Running: "bg-green-100 text-green-700",
  Pending: "bg-amber-100 text-amber-600",
  Failed: "bg-red-100 text-red-500",
  Succeeded: "bg-secondary/20 text-primary/60",
  Unknown: "bg-primary/10 text-primary/40",
};

const PodStatusBadge = ({ status }: { status: string }) => {
  const { t } = useTranslation();
  const color = statusColors[status] ?? statusColors["Unknown"];
  const label = t(`kubernetes.podStatuses.${status}`, { defaultValue: status });
  return (
    <span
      className={`inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-xs font-medium ${color}`}
    >
      <span
        className={`w-1.5 h-1.5 rounded-full ${
          status === "Running" ? "bg-green-500" : "bg-current opacity-60"
        }`}
      />
      {label}
    </span>
  );
};

interface DeploymentRowProps {
  deployment: DeploymentDto;
  onScale: (name: string, replicas: number) => Promise<void>;
  onRestart: (name: string) => void;
  onViewLogs: (name: string) => void;
}

const DeploymentRow = ({
  deployment: d,
  onScale,
  onRestart,
  onViewLogs
}: DeploymentRowProps) => {
  const { t } = useTranslation();
  const [replicas, setReplicas] = useState(d.replicas);
  const [scaling, setScaling] = useState(false);

  useEffect(() => {
    setReplicas(d.replicas);
  }, [d.replicas]);

  const handleScale = async () => {
    setScaling(true);
    try {
      await onScale(d.name, replicas);
    } finally {
      setScaling(false);
    }
  };

  const healthy =
    d.availableReplicas === d.replicas && d.readyReplicas === d.replicas;

  return (
    <tr className="border-b border-background/60 hover:bg-background/40 transition-colors">
      <td className="px-5 py-3.5 font-mono text-sm text-primary font-medium">{d.name}</td>
      <td className="px-5 py-3.5 text-sm text-primary">{d.replicas}</td>
      <td className="px-5 py-3.5 text-sm font-medium text-green-600">{d.readyReplicas}</td>
      <td className="px-5 py-3.5">
        <span className={`text-sm font-medium ${healthy ? 'text-green-600' : 'text-amber-500'}`}>
          {d.availableReplicas}
        </span>
      </td>
      {/* Replica stepper — без змін */}
      <td className="px-5 py-3.5">
        <div className="flex items-center gap-1.5">
          <button
            onClick={() => setReplicas(r => Math.max(0, r - 1))}
            className="w-6 h-6 flex items-center justify-center rounded-md border border-secondary/40 text-primary/60 hover:text-primary hover:border-primary transition-colors"
          >
            <Minus className="w-3 h-3" />
          </button>
          <input
            type="number" min={0} max={50} value={replicas}
            onChange={e => setReplicas(Math.max(0, Number(e.target.value)))}
            className="w-14 text-center border border-secondary/40 rounded-lg px-2 py-1 text-sm font-mono text-primary focus:outline-none focus:border-primary transition-colors"
          />
          <button
            onClick={() => setReplicas(r => r + 1)}
            className="w-6 h-6 flex items-center justify-center rounded-md border border-secondary/40 text-primary/60 hover:text-primary hover:border-primary transition-colors"
          >
            <Plus className="w-3 h-3" />
          </button>
        </div>
      </td>
      {/* Actions — додати View Logs */}
      <td className="px-5 py-3.5">
        <div className="flex items-center gap-2">
          <button
            onClick={handleScale}
            disabled={scaling || replicas === d.replicas}
            className="inline-flex items-center gap-1.5 px-3 py-1.5 bg-primary text-white text-xs font-medium rounded-xl hover:bg-primary-light transition-colors disabled:opacity-40"
          >
            {scaling
              ? <Loader2 className="w-3.5 h-3.5 animate-spin" />
              : t('kubernetes.scale')}
          </button>
          <button
            onClick={() => onRestart(d.name)}
            className="inline-flex items-center gap-1 px-3 py-1.5 border border-secondary/40 text-primary text-xs font-medium rounded-xl hover:bg-background transition-colors"
          >
            <RotateCcw className="w-3 h-3" />
            {t('kubernetes.restart')}
          </button>
          <button
            onClick={() => onViewLogs(d.name)}
            className="inline-flex items-center gap-1 px-3 py-1.5 border border-secondary/40 text-primary text-xs font-medium rounded-xl hover:bg-background transition-colors"
          >
            <Maximize2 className="w-3 h-3" />
            {t('kubernetes.viewLogs')}
          </button>
        </div>
      </td>
    </tr>
  );
};

interface LogsModalProps {
  target: { deploymentName: string } | null;
  onClose: () => void;
}

const LogsModal = ({ target, onClose }: LogsModalProps) => {
  const { t } = useTranslation();
  const [data, setData] = useState<PodLogsDto | null>(null);
  const [loading, setLoading] = useState(false);
  const bottomRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (!target) {
      setData(null);
      return;
    }
    setLoading(true);
    kubernetesApi
      .getLogs(target.deploymentName)
      .then(setData)
      .catch(() => toast.error(t("kubernetes.logsError")))
      .finally(() => setLoading(false));
  }, [target]);

  useEffect(() => {
    bottomRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [data]);

  const lines = data?.logs ? data.logs.split("\n") : [];

  const colorLine = (line: string) => {
    if (/\bERR\b|ERROR|CRITICAL/i.test(line)) return "text-red-400";
    if (/\bWRN\b|WARN/i.test(line)) return "text-amber-400";
    if (/\bINF\b|INFO/i.test(line)) return "text-secondary";
    if (/\bDBG\b|DEBUG/i.test(line)) return "text-primary/40";
    return "text-primary/70";
  };

  const title = data
    ? `${t("kubernetes.logsTitle")} — ${data.podName}`
    : t("kubernetes.logsTitle");

  return (
    <FormModal
      isOpen={!!target}
      title={title}
      onClose={onClose}
      width="max-w-3xl"
    >
      {loading ? (
        <LoadingSpinner />
      ) : lines.length === 0 ? (
        <p className="text-sm font-body text-primary/40 text-center py-6">
          {t("kubernetes.noLogs")}
        </p>
      ) : (
        <div className="bg-primary rounded-xl p-4 max-h-[60vh] overflow-y-auto font-mono text-xs leading-6">
          {lines.map((line, i) => (
            <p key={i} className={colorLine(line)}>
              {line || "\u00A0"}
            </p>
          ))}
          <div ref={bottomRef} />
        </div>
      )}
    </FormModal>
  );
};

const KubernetesPage = () => {
  const { t } = useTranslation();

  const [deployments, setDeployments] = useState<DeploymentDto[]>([]);
  const [pods, setPods] = useState<PodDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);

  const [restartTarget, setRestartTarget] = useState<string | null>(null);
  const [restarting, setRestarting] = useState(false);

  const [logsTarget, setLogsTarget] = useState<{
    deploymentName: string;
  } | null>(null);

  const load = async (silent = false) => {
    if (silent) setRefreshing(true);
    else setLoading(true);
    try {
      const [deps, podList] = await Promise.all([
        kubernetesApi.getDeployments(),
        kubernetesApi.getPods(),
      ]);
      setDeployments(deps);
      setPods(podList);
    } catch {
      toast.error(t("kubernetes.loadError"));
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  const handleScale = async (name: string, replicas: number) => {
    try {
      await kubernetesApi.scale({ deploymentName: name, replicas });
      toast.success(t("kubernetes.scaleSuccess"));
      load(true);
    } catch {
      toast.error(t("common.error"));
    }
  };

  const handleRestart = async () => {
    if (!restartTarget) return;
    setRestarting(true);
    try {
      await kubernetesApi.restart({ deploymentName: restartTarget });
      toast.success(t("kubernetes.restartSuccess"));
      load(true);
    } catch {
      toast.error(t("common.error"));
    } finally {
      setRestarting(false);
      setRestartTarget(null);
    }
  };

  const thClass =
    "text-left px-5 py-4 text-primary/50 font-medium text-xs uppercase tracking-wide whitespace-nowrap";

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="font-display text-3xl text-primary">
          {t("kubernetes.title")}
        </h1>
        <button
          onClick={() => load(true)}
          disabled={refreshing}
          className="inline-flex items-center gap-1.5 px-3 py-1.5 border border-secondary/40 text-primary text-sm font-medium rounded-xl hover:bg-background transition-colors disabled:opacity-50"
        >
          <RefreshCw
            className={`w-3.5 h-3.5 ${refreshing ? "animate-spin" : ""}`}
          />
          {t("common.refresh")}
        </button>
      </div>

      {loading ? (
        <LoadingSpinner />
      ) : (
        <div className="space-y-6">
          <div className="bg-white rounded-2xl shadow-card overflow-hidden">
            <div className="px-5 py-4 border-b border-background-muted">
              <h2 className="font-display text-xl text-primary">
                {t("kubernetes.deploymentsTitle")}
              </h2>
            </div>
            {deployments.length === 0 ? (
              <p className="text-sm text-primary/40 font-body text-center py-8">
                {t("common.noData")}
              </p>
            ) : (
              <div className="overflow-x-auto">
                <table className="w-full font-body">
                  <thead>
                    <tr className="border-b border-background-muted">
                      <th className={thClass}>
                        {t("kubernetes.colDeployment")}
                      </th>
                      <th className={thClass}>{t("kubernetes.colDesired")}</th>
                      <th className={thClass}>{t("kubernetes.colReady")}</th>
                      <th className={thClass}>
                        {t("kubernetes.colAvailable")}
                      </th>
                      <th className={thClass}>{t("kubernetes.colReplicas")}</th>
                      <th className={thClass}>{t("kubernetes.colActions")}</th>
                    </tr>
                  </thead>
                  <tbody>
                    {deployments.map((d) => (
                      <DeploymentRow
                        key={d.name}
                        deployment={d}
                        onScale={handleScale}
                        onRestart={setRestartTarget}
                        onViewLogs={(name) => setLogsTarget({ deploymentName: name })}
                      />
                    ))}
                  </tbody>
                </table>
              </div>
            )}
          </div>

          <div className="bg-white rounded-2xl shadow-card overflow-hidden">
            <div className="px-5 py-4 border-b border-background-muted">
              <h2 className="font-display text-xl text-primary">
                {t("kubernetes.podsTitle")}
              </h2>
            </div>
            {pods.length === 0 ? (
              <p className="text-sm text-primary/40 font-body text-center py-8">
                {t("common.noData")}
              </p>
            ) : (
              <div className="overflow-x-auto">
                <table className="w-full font-body">
                  <thead>
                    <tr className="border-b border-background-muted">
                      <th className={thClass}>{t("kubernetes.colPod")}</th>
                      <th className={thClass}>{t("kubernetes.colStatus")}</th>
                      <th className={thClass}>{t("kubernetes.colNode")}</th>
                      <th className={thClass}>{t("kubernetes.colIp")}</th>
                    </tr>
                  </thead>
                  <tbody>
                    {pods.map((pod) => (
                      <tr
                        key={pod.name}
                        className="border-b border-background/60 hover:bg-background/40 transition-colors"
                      >
                        <td className="px-5 py-3.5 font-mono text-sm text-primary">
                          {pod.name}
                        </td>
                        <td className="px-5 py-3.5">
                          <PodStatusBadge status={pod.status} />
                        </td>
                        <td className="px-5 py-3.5 text-sm text-primary/60 font-mono">
                          {pod.node}
                        </td>
                        <td className="px-5 py-3.5 text-sm text-primary/60 font-mono">
                          {pod.podIp}
                        </td> 
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}
          </div>

          <KubernetesConfigGenerator />
        </div>
      )}
      <ConfirmModal
        isOpen={!!restartTarget}
        message={t("kubernetes.confirmRestart", { name: restartTarget ?? "" })}
        onConfirm={handleRestart}
        onCancel={() => setRestartTarget(null)}
        loading={restarting}
      />

      <LogsModal target={logsTarget} onClose={() => setLogsTarget(null)} />
    </div>
  );
};

export default KubernetesPage;
