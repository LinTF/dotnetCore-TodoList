import axios from "axios";

let http = axios.create({
    // baseURL: 'https://localhost:7268/api/TodoItems/'
    baseURL: 'https://localhost:7268/api/TodoItems/'
});

// export const apiGet_todoList = () => http.get();
// export const apiPost_addTodoList = data => http.post('', data);
// export const apiPut_todoIsFinish = data => http.put(id + "/TodoItemDetails/" + data.id, data);
// export const apiPut_sortTodoItem = data => http.put("Sort", data);
// export const apiDelete_todoItem = data => http.delete(dateID + "/TodoItemDetails/" + itemID, data);
// export const apiDelete_emptyTodoList = data => http.delete("empty", data);

//// 有用到再打開改
// http.interceptors.request.use(
//     config => {
//         return config;
//     },
//     error => {
//         return Promise.reject(error);
//     }
// );

// http.interceptors.response.use(
//     response => {
//         return response;
//     },
//     error => {
//         return Promise.reject(error);
//     }
// );

export { http };