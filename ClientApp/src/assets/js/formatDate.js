export function formatDate(dateStr) {
  const [year, month, day] = dateStr.split('-');

  // 格式化日期
  return `${year}/${month}/${day}`;
};