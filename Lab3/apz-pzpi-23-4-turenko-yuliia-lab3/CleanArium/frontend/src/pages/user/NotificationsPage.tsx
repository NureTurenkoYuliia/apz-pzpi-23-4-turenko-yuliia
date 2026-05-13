import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { Bell, CheckCircle } from "lucide-react";
import toast from "react-hot-toast";
import { notificationApi } from "../../api/notification";
import { NotificationDto } from "../../types";
import LoadingSpinner from "../../components/common/LoadingSpinner";

const NotificationsPage = () => {
  const { t } = useTranslation();
  const [notifications, setNotifications] = useState<NotificationDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [marking, setMarking] = useState<number | null>(null);

  useEffect(() => {
    load();
  }, []);

  const load = async () => {
    setLoading(true);
    try {
      setNotifications(await notificationApi.getAll());
    } catch {
      toast.error(t("notification.loadError"));
    } finally {
      setLoading(false);
    }
  };

  const handleMarkRead = async (id: number) => {
    setMarking(id);
    try {
      await notificationApi.markAsRead(id);
      setNotifications((prev) =>
        prev.map((n) => (n.id === id ? { ...n, isRead: true } : n)),
      );
      toast.success(t("notification.markReadSuccess"));
    } catch {
      toast.error(t("common.error"));
    } finally {
      setMarking(null);
    }
  };

  const unreadCount = notifications.filter((n) => !n.isRead).length;

  return (
    <div>
      <div className="flex items-center gap-3 mb-6">
        <h1 className="font-display text-3xl text-primary">
          {t("notification.title")}
        </h1>
        {unreadCount > 0 && (
          <span className="px-2.5 py-0.5 bg-accent text-white text-xs font-bold rounded-full">
            {unreadCount}
          </span>
        )}
      </div>

      {loading ? (
        <LoadingSpinner />
      ) : notifications.length === 0 ? (
        <div className="bg-white rounded-2xl shadow-card p-12 text-center">
          <Bell className="w-10 h-10 text-primary/20 mx-auto mb-3" />
          <p className="text-primary/40 font-body">{t("notification.empty")}</p>
        </div>
      ) : (
        <div className="space-y-3">
          {notifications.map((n) => (
            <div
              key={n.id}
              className={`bg-white rounded-2xl shadow-card p-5 flex items-start justify-between gap-4 transition-opacity ${n.isRead ? "opacity-60" : ""}`}
            >
              <div className="flex items-start gap-3">
                {!n.isRead && (
                  <span className="mt-1.5 w-2 h-2 bg-accent rounded-full flex-shrink-0" />
                )}
                <div className={n.isRead ? "pl-5" : ""}>
                  <p className="font-body font-semibold text-primary text-sm">
                    {n.title}
                  </p>
                  <p className="font-body text-primary/60 text-sm mt-0.5">
                    {n.content}
                  </p>
                  <p className="font-body text-primary/30 text-xs mt-1.5">
                    {new Date(n.createdAt).toLocaleString()}
                  </p>
                </div>
              </div>
              {!n.isRead && (
                <button
                  onClick={() => handleMarkRead(n.id)}
                  disabled={marking === n.id}
                  title={t("notification.markRead")}
                  className="flex-shrink-0 p-1.5 rounded-lg text-primary/30 hover:text-primary hover:bg-background transition-colors disabled:opacity-40"
                >
                  <CheckCircle className="w-4 h-4" />
                </button>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default NotificationsPage;
