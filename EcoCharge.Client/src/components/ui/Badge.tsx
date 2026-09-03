import type { StationStatus } from '@/types/station';

const STATUS_STYLES: Record<StationStatus, string> = {
  Available: 'bg-brand-100 text-brand-800 ring-1 ring-inset ring-brand-300',
  Charging: 'bg-amber-100 text-amber-800 ring-1 ring-inset ring-amber-300',
  Maintenance: 'bg-graphite-200 text-graphite-700 ring-1 ring-inset ring-graphite-300',
};

const STATUS_LABELS: Record<StationStatus, string> = {
  Available: 'Available',
  Charging: 'Charging',
  Maintenance: 'Maintenance',
};

interface StatusBadgeProps {
  status: StationStatus;
}

export function StatusBadge({ status }: StatusBadgeProps) {
  return (
    <span
      className={`inline-flex items-center gap-1.5 rounded-full px-2.5 py-1 text-xs font-semibold ${STATUS_STYLES[status]}`}
    >
      <span className="h-1.5 w-1.5 rounded-full bg-current" />
      {STATUS_LABELS[status]}
    </span>
  );
}
