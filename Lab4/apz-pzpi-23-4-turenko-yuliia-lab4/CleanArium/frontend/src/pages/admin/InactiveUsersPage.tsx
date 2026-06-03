import { useState } from "react";
import { useTranslation } from "react-i18next";
import toast from "react-hot-toast";
import { userApi } from "../../api/users";
import { InactiveUserDto } from "../../types";
import LoadingSpinner from "../../components/common/LoadingSpinner";

const InactiveUsersPage = () => {
  const { t } = useTranslation();
  const [days, setDays] = useState(60);
  const [users, setUsers] = useState<InactiveUserDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [fetched, setFetched] = useState(false);

  const fetch = async () => {
    setLoading(true);
    try {
      const data = await userApi.getInactiveUsers(days);
      setUsers(data);
      setFetched(true);
    } catch {
      toast.error(t("users.loadError"));
    } finally {
      setLoading(false);
    }
  };

  const fmt = (d: string | null) =>
    d ? new Date(d).toLocaleDateString() : t("users.never");

  return (
    <div>
      <div className="mb-6">
        <h1 className="font-display text-3xl text-primary">
          {t("users.inactiveUsers")}
        </h1>
      </div>

      <div className="bg-white rounded-2xl shadow-card p-5 mb-5 flex items-center gap-4">
        <label className="text-sm font-body text-primary/60">
          {t("users.inactiveDays")}
        </label>
        <input
          type="number"
          min={1}
          max={365}
          value={days}
          onChange={(e) => setDays(Number(e.target.value))}
          className="w-24 border border-secondary/40 rounded-lg px-3 py-1.5 text-sm font-body text-primary focus:outline-none focus:border-primary transition-colors"
        />
        <button
          onClick={fetch}
          disabled={loading}
          className="px-4 py-1.5 bg-primary text-white text-sm font-body font-medium rounded-xl hover:bg-primary-light transition-colors disabled:opacity-50"
        >
          {loading ? t("common.loading") : t("users.fetch")}
        </button>
      </div>

      {loading ? (
        <LoadingSpinner />
      ) : fetched && users.length === 0 ? (
        <p className="text-sm font-body text-primary/40 text-center py-8">
          {t("users.noUsers")}
        </p>
      ) : fetched ? (
        <div className="bg-white rounded-2xl shadow-card overflow-hidden">
          <table className="w-full text-sm font-body">
            <thead>
              <tr className="border-b border-background-muted">
                {[
                  "users.userId",
                  "users.email",
                  "users.lastLogin",
                  "users.aquariumsCount",
                  "users.activeAquariums",
                  "users.activeDevices",
                ].map((k) => (
                  <th
                    key={k}
                    className="text-left px-5 py-4 text-primary/50 font-medium text-xs uppercase tracking-wide"
                  >
                    {t(k)}
                  </th>
                ))}
              </tr>
            </thead>
            <tbody>
              {users.map((u) => (
                <tr
                  key={u.userId}
                  className="border-b border-background/60 hover:bg-background/40 transition-colors"
                >
                  <td className="px-5 py-3.5 font-mono text-primary/70">
                    {u.userId}
                  </td>
                  <td className="px-5 py-3.5 text-primary">{u.email}</td>
                  <td className="px-5 py-3.5 text-primary/60">
                    {fmt(u.lastLoginAt)}
                  </td>
                  <td className="px-5 py-3.5 text-primary">
                    {u.aquariumsCount}
                  </td>
                  <td className="px-5 py-3.5 text-primary">
                    {u.activeAquariums}
                  </td>
                  <td className="px-5 py-3.5 text-primary">
                    {u.activeDevices}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      ) : null}
    </div>
  );
};

export default InactiveUsersPage;
