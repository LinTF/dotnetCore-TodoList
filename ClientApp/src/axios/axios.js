import axios from 'axios'

const API = axios.create({
    // baseURL: 'https://localhost:{port}',
    baseURL: 'https://localhost:7268',
    timeout:2000
})

export default API;