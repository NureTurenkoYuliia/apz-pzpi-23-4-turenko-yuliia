import { useEffect, useState } from "react";
import { useNavigate, useParams, Link } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { Plus, Pencil, Trash2, ChevronRight, ArrowLeft } from "lucide-react";
import toast from "react-hot-toast";
import { deviceApi } from "../../api/device";
import { DeviceDto, DeviceStatus, DeviceType } from "../../types";
import LoadingSpinner from "../../components/common/LoadingSpinner";
import ConfirmModal from "../../components/modals/ConfirmModal";
import FormModal from "../../components/common/FormModal";

const schema = z.object({
  deviceType: z.coerce.number().int().min(1),
  deviceStatus: z.coerce.number().int().min(1),
});
type FormValues = { deviceType: number; deviceStatus: number };

const DevicesPage = () => {
  const { t } = useTranslation();
  const { aquariumId } = useParams<{ aquariumId: string }>();
  const navigate = useNavigate();
  const aqId = Number(aquariumId);

  const [devices, setDevices] = useState<DeviceDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<DeviceDto | null>(null);
  const [deleteId, setDeleteId] = useState<number | null>(null);
  const [deleting, setDeleting] = useState(false);
  const [submitting, setSubmitting] = useState(false);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: {
      deviceType: DeviceType.Heater,
      deviceStatus: DeviceStatus.Off,
    },
  });

  useEffect(() => {
    load();
  }, [aqId]);

  const load = async () => {
    setLoading(true);
    try {
      setDevices(await deviceApi.getByAquarium(aqId));
    } catch {
      toast.error(t("device.loadError"));
    } finally {
      setLoading(false);
    }
  };

  const openCreate = () => {
    setEditing(null);
    reset({ deviceType: DeviceType.Heater, deviceStatus: DeviceStatus.Off });
    setModalOpen(true);
  };
  const openEdit = (d: DeviceDto) => {
    setEditing(d);
    reset({ deviceType: d.deviceType, deviceStatus: d.deviceStatus });
    setModalOpen(true);
  };

  const onSubmit = async (values: FormValues) => {
    setSubmitting(true);
    try {
      if (editing) {
        await deviceApi.update({
          deviceId: editing.id,
          deviceType: values.deviceType,
          deviceStatus: values.deviceStatus,
        });
        toast.success(t("device.updateSuccess"));
        setDevices((prev) =>
          prev.map((d) => (d.id === editing.id ? { ...d, ...values } : d)),
        );
      } else {
        const id = await deviceApi.create({
          aquariumId: aqId,
          deviceType: values.deviceType,
          deviceStatus: values.deviceStatus,
        });
        toast.success(t("device.createSuccess"));
        setDevices((prev) => [
          ...prev,
          {
            id,
            aquariumId: aqId,
            deviceType: values.deviceType,
            deviceStatus: values.deviceStatus,
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
      await deviceApi.delete(deleteId);
      setDevices((prev) => prev.filter((d) => d.id !== deleteId));
      toast.success(t("device.deleteSuccess"));
    } catch {
      toast.error(t("common.error"));
    } finally {
      setDeleting(false);
      setDeleteId(null);
    }
  };

  const deviceTypeOptions = Object.entries({
    1: t("device.deviceTypes.1"),
    2: t("device.deviceTypes.2"),
    3: t("device.deviceTypes.3"),
    4: t("device.deviceTypes.4"),
    5: t("device.deviceTypes.5"),
  });
  const statusOptions = Object.entries({
    1: t("device.deviceStatuses.1"),
    2: t("device.deviceStatuses.2"),
    3: t("device.deviceStatuses.3"),
  });

  const statusColor: Record<number, string> = {
    1: "bg-green-100 text-green-700",
    2: "bg-primary/10 text-primary/50",
    3: "bg-amber-100 text-amber-600",
  };

  return (
    <div>
      <Link
        to={`/user/aquariums`}
        className="inline-flex items-center gap-1 text-sm text-primary/50 hover:text-primary transition-colors mb-5"
      >
        <ArrowLeft className="w-3.5 h-3.5" /> {t("device.backToAquariums")}
      </Link>

      <div className="flex items-center justify-between mb-6">
        <h1 className="font-display text-3xl text-primary">
          {t("device.title")}
        </h1>
        <button
          onClick={openCreate}
          className="inline-flex items-center gap-1.5 px-4 py-1.5 bg-primary text-white text-sm font-medium rounded-xl hover:bg-primary-light transition-colors"
        >
          <Plus className="w-3.5 h-3.5" /> {t("common.add")}
        </button>
      </div>

      {loading ? (
        <LoadingSpinner />
      ) : devices.length === 0 ? (
        <div className="bg-white rounded-2xl shadow-card p-12 text-center">
          <p className="text-primary/40 font-body">{t("device.empty")}</p>
        </div>
      ) : (
        <div className="grid gap-4 sm:grid-cols-2">
          {devices.map((d) => (
            <div
              key={d.id}
              className="bg-white rounded-2xl shadow-card p-5 hover:shadow-card-hover transition-shadow"
            >
              <div className="flex items-start justify-between mb-3">
                <div>
                  <p className="font-display text-lg text-primary">
                    {t(`device.deviceTypes.${d.deviceType}`)}
                  </p>
                </div>
                <span
                  className={`text-xs font-medium px-2 py-0.5 rounded-full ${statusColor[d.deviceStatus] ?? ""}`}
                >
                  {t(`device.deviceStatuses.${d.deviceStatus}`)}
                </span>
              </div>
              <div className="flex items-center gap-2 pt-3 border-t border-background">
                <button
                  onClick={() => navigate(`devices/${d.id}`)}
                  className="flex-1 flex items-center justify-center gap-1 py-1.5 text-sm font-medium text-primary hover:text-primary-light transition-colors"
                >
                  {t("device.viewDetail")}{" "}
                  <ChevronRight className="w-3.5 h-3.5" />
                </button>
                <button
                  onClick={() => openEdit(d)}
                  className="p-1.5 rounded-lg text-primary/40 hover:text-primary hover:bg-background transition-colors"
                >
                  <Pencil className="w-4 h-4" />
                </button>
                <button
                  onClick={() => setDeleteId(d.id)}
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
        title={t(editing ? "device.editTitle" : "device.createTitle")}
        onClose={() => setModalOpen(false)}
      >
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div>
            <label className="block text-sm font-body font-medium text-primary/70 mb-1.5">
              {t("device.type")}
            </label>
            <select
              {...register("deviceType")}
              className="w-full border border-secondary/40 rounded-xl px-4 py-2.5 text-sm font-body text-primary focus:outline-none focus:border-primary bg-white"
            >
              {deviceTypeOptions.map(([v, label]) => (
                <option key={v} value={v}>
                  {label}
                </option>
              ))}
            </select>
            {errors.deviceType && (
              <p className="text-red-500 text-xs mt-1">
                {errors.deviceType.message}
              </p>
            )}
          </div>
          <div>
            <label className="block text-sm font-body font-medium text-primary/70 mb-1.5">
              {t("device.status")}
            </label>
            <select
              {...register("deviceStatus")}
              className="w-full border border-secondary/40 rounded-xl px-4 py-2.5 text-sm font-body text-primary focus:outline-none focus:border-primary bg-white"
            >
              {statusOptions.map(([v, label]) => (
                <option key={v} value={v}>
                  {label}
                </option>
              ))}
            </select>
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
        message={t("device.confirmDelete")}
        onConfirm={handleDelete}
        onCancel={() => setDeleteId(null)}
        danger
        loading={deleting}
      />
    </div>
  );
};

export default DevicesPage;
