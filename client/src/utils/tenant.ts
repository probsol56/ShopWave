export const getTenantSlug = (): string | null => {
  const host = window.location.hostname;
  const parts = host.split('.');
  if (parts.length >= 3) return parts[0];
  return null;
};
