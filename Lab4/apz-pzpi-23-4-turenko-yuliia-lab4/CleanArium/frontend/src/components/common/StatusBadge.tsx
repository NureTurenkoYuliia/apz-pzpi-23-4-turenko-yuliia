interface Props {
  active: boolean;
  activeLabel: string;
  inactiveLabel: string;
}

const StatusBadge = ({ active, activeLabel, inactiveLabel }: Props) => (
  <span
    className={`inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-xs font-medium ${
      active ? 'bg-green-100 text-green-700' : 'bg-primary/10 text-primary/50'
    }`}
  >
    <span className={`w-1.5 h-1.5 rounded-full ${active ? 'bg-green-500' : 'bg-primary/30'}`} />
    {active ? activeLabel : inactiveLabel}
  </span>
);

export default StatusBadge;
