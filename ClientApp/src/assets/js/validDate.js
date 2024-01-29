export function validDate(dateString) {
    // 將正確日期格式拆開
    const [year, month, day] = dateString.split('-');
            
    // 檢查年份是否不小於 1911
    if (year < 1911) {
        return false;
    }

    // 檢查月份是否在 1 到 12 的範圍內
    if (month < 1 || month > 12) {
        return false;
    }

    // 檢查日期是否在合法範圍內
    if (day < 1 || day > 31) {
        return false;
    }

    return true;
};