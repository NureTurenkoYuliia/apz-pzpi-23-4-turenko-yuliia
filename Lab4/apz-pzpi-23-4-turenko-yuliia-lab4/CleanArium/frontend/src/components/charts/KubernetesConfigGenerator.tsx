import { useState } from "react";
import { useTranslation } from "react-i18next";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { Check, Copy, Loader2 } from "lucide-react";
import toast from "react-hot-toast";
import { kubernetesApi } from "../../api/kubernetes";
import { KubernetesConfigResponse } from "../../types";

// ─── Zod schema ───────────────────────────────────────────────────────────────

const schema = (t: (k: string) => string) =>
  z
    .object({
      appName: z
        .string()
        .min(1, t("validation.required"))
        .max(63)
        .regex(/^[a-z0-9-]+$/, "lowercase letters, numbers and hyphens only"),
      image: z.string().min(1, t("validation.required")),
      replicas: z.coerce.number().int().min(1).max(50),
      containerPort: z.coerce.number().int().min(1).max(65535),
      cpuRequest: z.string().min(1, t("validation.required")),
      memoryRequest: z.string().min(1, t("validation.required")),
      minReplicas: z.coerce.number().int().min(1),
      maxReplicas: z.coerce.number().int().min(1),
    })
    .refine((d) => d.maxReplicas >= d.minReplicas, {
      path: ["maxReplicas"],
      message: t("kubernetes.validationMaxReplicas"),
    });

type FormValues = {
  appName: string;
  image: string;
  replicas: number;
  containerPort: number;
  cpuRequest: string;
  memoryRequest: string;
  minReplicas: number;
  maxReplicas: number;
};

// ─── YAML tab viewer ──────────────────────────────────────────────────────────

type TabKey = "deployment" | "service" | "hpa";

interface YamlViewerProps {
  result: KubernetesConfigResponse;
}

const YamlViewer = ({ result }: YamlViewerProps) => {
  const { t } = useTranslation();
  const [activeTab, setActiveTab] = useState<TabKey>("deployment");
  const [copiedTab, setCopiedTab] = useState<TabKey | null>(null);

  const tabs: { key: TabKey; label: string; content: string }[] = [
    {
      key: "deployment",
      label: t("kubernetes.tabDeployment"),
      content: result.deploymentYaml,
    },
    {
      key: "service",
      label: t("kubernetes.tabService"),
      content: result.serviceYaml,
    },
    { key: "hpa", label: t("kubernetes.tabHpa"), content: result.hpaYaml },
  ];

  const handleCopy = async (content: string, key: TabKey) => {
    try {
      await navigator.clipboard.writeText(content);
      setCopiedTab(key);
      setTimeout(() => setCopiedTab(null), 2000);
    } catch {
      toast.error(t("common.error"));
    }
  };

  const active = tabs.find((t) => t.key === activeTab)!;

  return (
    <div className="mt-6 border border-secondary/30 rounded-2xl overflow-hidden">
      {/* Tab bar */}
      <div className="flex items-center justify-between bg-background-muted px-4 pt-1 border-b border-secondary/20">
        <div className="flex gap-0.5">
          {tabs.map((tab) => (
            <button
              key={tab.key}
              onClick={() => setActiveTab(tab.key)}
              className={`px-4 py-2.5 text-xs font-mono font-medium rounded-t-lg transition-colors border-b-2 ${
                activeTab === tab.key
                  ? "border-primary text-primary bg-white"
                  : "border-transparent text-primary/50 hover:text-primary hover:bg-white/60"
              }`}
            >
              {tab.label}
            </button>
          ))}
        </div>

        {/* Copy button for active tab */}
        <button
          onClick={() => handleCopy(active.content, active.key)}
          className="inline-flex items-center gap-1.5 px-3 py-1.5 mb-1 text-xs font-medium text-primary border border-secondary/40 rounded-lg hover:bg-white transition-colors"
        >
          {copiedTab === active.key ? (
            <>
              <Check className="w-3 h-3 text-green-500" />
              {t("kubernetes.copied")}
            </>
          ) : (
            <>
              <Copy className="w-3 h-3" />
              {t("kubernetes.copy")}
            </>
          )}
        </button>
      </div>

      {/* YAML content */}
      <div className="bg-primary overflow-auto max-h-80">
        <pre className="p-5 text-xs font-mono leading-6 text-secondary/90 whitespace-pre">
          {active.content}
        </pre>
      </div>
    </div>
  );
};

// ─── Field helper ─────────────────────────────────────────────────────────────

interface FieldProps {
  label: string;
  error?: string;
  children: React.ReactNode;
  half?: boolean;
}

const Field = ({ label, error, children, half = false }: FieldProps) => (
  <div className={half ? "" : "col-span-2 sm:col-span-1"}>
    <label className="block text-sm font-body font-medium text-primary/70 mb-1.5">
      {label}
    </label>
    {children}
    {error && <p className="text-red-500 text-xs mt-1">{error}</p>}
  </div>
);

const inputClass =
  "w-full border border-secondary/40 rounded-xl px-4 py-2.5 text-sm font-body text-primary placeholder-primary/30 focus:outline-none focus:border-primary transition-colors";

// ─── Main component ───────────────────────────────────────────────────────────

const KubernetesConfigGenerator = () => {
  const { t } = useTranslation();
  const [result, setResult] = useState<KubernetesConfigResponse | null>(null);
  const [generating, setGenerating] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FormValues>({
    resolver: zodResolver(schema(t)),
    defaultValues: {
      replicas: 1,
      containerPort: 8080,
      cpuRequest: "100m",
      memoryRequest: "128Mi",
      minReplicas: 1,
      maxReplicas: 5,
    },
  });

  const onSubmit = async (values: FormValues) => {
    setGenerating(true);
    setResult(null);
    try {
      const data = await kubernetesApi.generateConfig(values);
      setResult(data);
    } catch {
      toast.error(t("kubernetes.generateError"));
    } finally {
      setGenerating(false);
    }
  };

  return (
    <div className="bg-white rounded-2xl shadow-card overflow-hidden">
      {/* Section header */}
      <div className="px-6 py-4 border-b border-background-muted">
        <h2 className="font-display text-xl text-primary">
          {t("kubernetes.configGenerator")}
        </h2>
      </div>

      <div className="p-6">
        <form onSubmit={handleSubmit(onSubmit)}>
          {/* 2-column grid */}
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-x-5 gap-y-4">
            {/* App name – full width */}
            <div className="sm:col-span-2">
              <Field
                label={t("kubernetes.appName")}
                error={errors.appName?.message}
              >
                <input
                  {...register("appName")}
                  placeholder={t("kubernetes.appNamePlaceholder")}
                  className={inputClass}
                />
              </Field>
            </div>

            {/* Docker image – full width */}
            <div className="sm:col-span-2">
              <Field
                label={t("kubernetes.image")}
                error={errors.image?.message}
              >
                <input
                  {...register("image")}
                  placeholder={t("kubernetes.imagePlaceholder")}
                  className={inputClass}
                />
              </Field>
            </div>

            <Field
              label={t("kubernetes.replicas")}
              error={errors.replicas?.message}
            >
              <input
                type="number"
                min={1}
                max={50}
                {...register("replicas")}
                className={inputClass}
              />
            </Field>

            <Field
              label={t("kubernetes.port")}
              error={errors.containerPort?.message}
            >
              <input
                type="number"
                min={1}
                max={65535}
                {...register("containerPort")}
                className={inputClass}
              />
            </Field>

            <Field
              label={t("kubernetes.cpuRequest")}
              error={errors.cpuRequest?.message}
            >
              <input
                {...register("cpuRequest")}
                placeholder={t("kubernetes.cpuPlaceholder")}
                className={inputClass}
              />
            </Field>

            <Field
              label={t("kubernetes.memoryRequest")}
              error={errors.memoryRequest?.message}
            >
              <input
                {...register("memoryRequest")}
                placeholder={t("kubernetes.memoryPlaceholder")}
                className={inputClass}
              />
            </Field>

            <Field
              label={t("kubernetes.minReplicas")}
              error={errors.minReplicas?.message}
            >
              <input
                type="number"
                min={1}
                {...register("minReplicas")}
                className={inputClass}
              />
            </Field>

            <Field
              label={t("kubernetes.maxReplicas")}
              error={errors.maxReplicas?.message}
            >
              <input
                type="number"
                min={1}
                {...register("maxReplicas")}
                className={inputClass}
              />
            </Field>
          </div>

          {/* Submit */}
          <div className="mt-6 flex justify-end">
            <button
              type="submit"
              disabled={generating}
              className="inline-flex items-center gap-2 px-6 py-2.5 bg-primary text-white text-sm font-medium rounded-xl hover:bg-primary-light transition-colors disabled:opacity-60"
            >
              {generating && <Loader2 className="w-3.5 h-3.5 animate-spin" />}
              {generating
                ? t("kubernetes.generating")
                : t("kubernetes.generateConfig")}
            </button>
          </div>
        </form>

        {/* Result */}
        {result && <YamlViewer result={result} />}
      </div>
    </div>
  );
};

export default KubernetesConfigGenerator;
