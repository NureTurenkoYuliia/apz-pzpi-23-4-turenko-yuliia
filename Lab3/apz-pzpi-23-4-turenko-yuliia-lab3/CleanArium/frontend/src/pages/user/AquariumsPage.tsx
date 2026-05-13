import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import {
  Plus,
  Pencil,
  Trash2,
  ChevronRight,
  Download,
  ChevronDown,
} from "lucide-react";
import toast from "react-hot-toast";
import { aquariumApi } from "../../api/aquarium";
import { AquariumDto } from "../../types";
import LoadingSpinner from "../../components/common/LoadingSpinner";
import ConfirmModal from "../../components/modals/ConfirmModal";
import FormModal from "../../components/common/FormModal";
import StatusBadge from "../../components/common/StatusBadge";
import FileImportButton from "../../components/common/FileImportButton";
import { downloadBlob } from "../../utils/downloadBlob";

const schema = (t: (k: string) => string) =>
  z.object({
    name: z.string().min(1, t("validation.required")).max(100),
    location: z.string().max(200).optional(),
  });
type FormValues = { name: string; location?: string };

const AquariumsPage = () => {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const [aquariums, setAquariums] = useState<AquariumDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<AquariumDto | null>(null);
  const [deleteId, setDeleteId] = useState<number | null>(null);
  const [deleting, setDeleting] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [exportOpen, setExportOpen] = useState(false);
  const [importing, setImporting] = useState(false);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<FormValues>({
    resolver: zodResolver(schema(t)),
  });

  useEffect(() => {
    load();
  }, []);

  const load = async () => {
    setLoading(true);
    try {
      setAquariums(await aquariumApi.getAll());
    } catch {
      toast.error(t("aquarium.loadError"));
    } finally {
      setLoading(false);
    }
  };

  const openCreate = () => {
    setEditing(null);
    reset({ name: "", location: "" });
    setModalOpen(true);
  };
  const openEdit = (a: AquariumDto) => {
    setEditing(a);
    reset({ name: a.name, location: a.location ?? "" });
    setModalOpen(true);
  };

  const onSubmit = async (values: FormValues) => {
    setSubmitting(true);
    try {
      if (editing) {
        await aquariumApi.update({
          aquariumId: editing.id,
          name: values.name,
          location: values.location,
        });
        toast.success(t("aquarium.updateSuccess"));
        setAquariums((prev) =>
          prev.map((a) => (a.id === editing.id ? { ...a, ...values } : a)),
        );
      } else {
        const id = await aquariumApi.create({
          name: values.name,
          location: values.location,
        });
        toast.success(t("aquarium.createSuccess"));
        setAquariums((prev) => [
          ...prev,
          {
            id,
            userId: 0,
            name: values.name,
            location: values.location ?? null,
            isActive: true,
          },
        ]);
      }
      setModalOpen(false);
    } catch {
      toast.error(t("common.error"));
    } finally {
      setSubmitting(false);
    }
  };

  const handleDelete = async () => {
    if (!deleteId) return;
    setDeleting(true);
    try {
      await aquariumApi.delete(deleteId);
      setAquariums((prev) => prev.filter((a) => a.id !== deleteId));
      toast.success(t("aquarium.deleteSuccess"));
    } catch {
      toast.error(t("common.error"));
    } finally {
      setDeleting(false);
      setDeleteId(null);
    }
  };

  const handleExport = async (type: "csv" | "json" | "pdf") => {
    setExportOpen(false);
    try {
      const blob =
        type === "csv"
          ? await aquariumApi.exportCsv()
          : type === "json"
            ? await aquariumApi.exportJson()
            : await aquariumApi.exportPdf();
      const ext = type;
      downloadBlob(blob, `aquariums.${ext}`);
    } catch {
      toast.error(t("common.error"));
    }
  };

  const handleImport = async (file: File) => {
    setImporting(true);
    try {
      await aquariumApi.import(file);
      toast.success(t("aquarium.importSuccess"));
      await load();
    } catch {
      toast.error(t("aquarium.importError"));
    } finally {
      setImporting(false);
    }
  };

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="font-display text-3xl text-primary">
          {t("aquarium.title")}
        </h1>
        <div className="flex items-center gap-2">
          <div className="relative">
            <button
              onClick={() => setExportOpen((o) => !o)}
              className="inline-flex items-center gap-1.5 px-3 py-1.5 text-sm font-medium text-primary border border-secondary/40 rounded-xl hover:bg-background transition-colors"
            >
              <Download className="w-3.5 h-3.5" />
              {t("common.export")}
              <ChevronDown className="w-3 h-3" />
            </button>
            {exportOpen && (
              <>
                <div
                  className="fixed inset-0 z-10"
                  onClick={() => setExportOpen(false)}
                />
                <div className="absolute right-0 top-10 z-20 bg-white rounded-xl shadow-modal border border-background-muted py-1 min-w-36">
                  {(["csv", "json", "pdf"] as const).map((type) => (
                    <button
                      key={type}
                      onClick={() => handleExport(type)}
                      className="w-full text-left px-4 py-2 text-sm text-primary hover:bg-background transition-colors uppercase font-mono"
                    >
                      {t(
                        `aquarium.export${type.charAt(0).toUpperCase() + type.slice(1)}` as `aquarium.exportCsv`,
                      )}
                    </button>
                  ))}
                </div>
              </>
            )}
          </div>

          <FileImportButton
            onFile={handleImport}
            accept=".json,.csv"
            label={t("aquarium.importFile")}
            loading={importing}
          />

          <button
            onClick={openCreate}
            className="inline-flex items-center gap-1.5 px-4 py-1.5 bg-primary text-white text-sm font-medium rounded-xl hover:bg-primary-light transition-colors"
          >
            <Plus className="w-3.5 h-3.5" /> {t("common.add")}
          </button>
        </div>
      </div>

      {loading ? (
        <LoadingSpinner />
      ) : aquariums.length === 0 ? (
        <div className="bg-white rounded-2xl shadow-card p-12 text-center">
          <p className="text-primary/40 font-body">{t("aquarium.empty")}</p>
          <button
            onClick={openCreate}
            className="mt-4 px-5 py-2 bg-primary text-white text-sm font-medium rounded-xl hover:bg-primary-light transition-colors"
          >
            <Plus className="w-3.5 h-3.5 inline mr-1" />
            {t("common.add")}
          </button>
        </div>
      ) : (
        <div className="grid gap-4 sm:grid-cols-2">
          {aquariums.map((a) => (
            <div
              key={a.id}
              className="bg-white rounded-2xl shadow-card p-5 flex flex-col gap-3 hover:shadow-card-hover transition-shadow"
            >
              <div className="flex items-start justify-between gap-2">
                <div>
                  <h2 className="font-display text-lg text-primary">
                    {a.name}
                  </h2>
                  {a.location && (
                    <p className="text-xs font-body text-primary/50 mt-0.5">
                      {a.location}
                    </p>
                  )}
                </div>
                <StatusBadge
                  active={a.isActive}
                  activeLabel={t("common.active")}
                  inactiveLabel={t("common.inactive")}
                />
              </div>
              <div className="flex items-center gap-2 mt-auto pt-2 border-t border-background">
                <button
                  onClick={() => navigate(`${a.id}`)}
                  className="flex-1 flex items-center justify-center gap-1 py-1.5 text-sm font-medium text-primary hover:text-primary-light transition-colors"
                >
                  {t("aquarium.viewDevices")}{" "}
                  <ChevronRight className="w-3.5 h-3.5" />
                </button>
                <button
                  onClick={() => openEdit(a)}
                  className="p-1.5 rounded-lg text-primary/40 hover:text-primary hover:bg-background transition-colors"
                >
                  <Pencil className="w-4 h-4" />
                </button>
                <button
                  onClick={() => setDeleteId(a.id)}
                  className="p-1.5 rounded-lg text-primary/40 hover:text-red-500 hover:bg-red-50 transition-colors"
                >
                  <Trash2 className="w-4 h-4" />
                </button>
              </div>
            </div>
          ))}
        </div>
      )}

      <FormModal
        isOpen={modalOpen}
        title={t(editing ? "aquarium.editTitle" : "aquarium.createTitle")}
        onClose={() => setModalOpen(false)}
      >
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div>
            <label className="block text-sm font-body font-medium text-primary/70 mb-1.5">
              {t("aquarium.name")}
            </label>
            <input
              {...register("name")}
              placeholder={t("aquarium.namePlaceholder")}
              className="w-full border border-secondary/40 rounded-xl px-4 py-2.5 text-sm font-body text-primary focus:outline-none focus:border-primary transition-colors"
            />
            {errors.name && (
              <p className="text-red-500 text-xs mt-1">{errors.name.message}</p>
            )}
          </div>
          <div>
            <label className="block text-sm font-body font-medium text-primary/70 mb-1.5">
              {t("aquarium.location")}
            </label>
            <input
              {...register("location")}
              placeholder={t("aquarium.locationPlaceholder")}
              className="w-full border border-secondary/40 rounded-xl px-4 py-2.5 text-sm font-body text-primary focus:outline-none focus:border-primary transition-colors"
            />
          </div>
          <div className="flex justify-end gap-3 pt-2">
            <button
              type="button"
              onClick={() => setModalOpen(false)}
              className="px-4 py-2 text-sm text-primary/60 hover:text-primary transition-colors"
            >
              {t("common.cancel")}
            </button>
            <button
              type="submit"
              disabled={submitting}
              className="px-5 py-2 bg-primary text-white text-sm font-medium rounded-xl hover:bg-primary-light transition-colors disabled:opacity-60"
            >
              {submitting ? t("common.saving") : t("common.save")}
            </button>
          </div>
        </form>
      </FormModal>

      <ConfirmModal
        isOpen={!!deleteId}
        message={t("aquarium.confirmDelete")}
        onConfirm={handleDelete}
        onCancel={() => setDeleteId(null)}
        danger
        loading={deleting}
      />
    </div>
  );
};

export default AquariumsPage;
