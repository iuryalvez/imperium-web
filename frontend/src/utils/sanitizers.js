export const sanitizeToNumbers = (value) => {
  if (typeof value !== 'string') return '';
  return value.replace(/\D/g, '');
};