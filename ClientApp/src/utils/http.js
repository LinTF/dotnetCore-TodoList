import axios from "axios";

let http = axios.create({
    // baseURL: 'https://localhost:7268/api/TodoItems/'
    baseURL: 'https://localhost:7268/api/'
});

export { http };