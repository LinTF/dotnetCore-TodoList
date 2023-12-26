import axios from 'axios'

const API = axios.create({
    baseURL: 'https://localhost:{port}',
    timeout:2000
})

export default API;