import { useTranslation } from "react-i18next";
import DailyActivityChart from "../../components/charts/DailyActivityChart";
import CorrelationTable from "../../components/charts/CorrelationTable";
import DeviceRulesPanel from "../../components/charts/DeviceRulesPanel";

const AnalyticsPage = () => {
  const { t } = useTranslation();
  return (
    <div>
      <div className="mb-6">
        <h1 className="font-display text-3xl text-primary">
          {t("analytics.title")}
        </h1>
      </div>
      <div className="space-y-6">
        <DailyActivityChart />
        <CorrelationTable />
        <DeviceRulesPanel />
      </div>
    </div>
  );
};

export default AnalyticsPage;
