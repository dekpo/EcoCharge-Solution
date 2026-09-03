import type { PropsWithChildren } from 'react';

interface CardProps extends PropsWithChildren {
  className?: string;
}

export function Card({ children, className = '' }: CardProps) {
  return (
    <div className={`rounded-2xl border border-graphite-100 bg-white p-5 shadow-sm ${className}`}>{children}</div>
  );
}
