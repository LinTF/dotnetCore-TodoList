import { createStore } from 'vuex'
import { http } from '@/utils/http'

export default createStore({
    state: {
        todoData: []
    },
    getters: {
    },
    mutations: {
        setTodoData(state, payload) {
            for (let todoGroupb of payload) {
                todoGroupb.isEdit = false;
            }
            state.todoData = payload;
        }
    },
    actions: {
        async Api_GetTodoData(context, payload) {
            const { data } = await http.get("/");
            context.commit('setTodoData', data);
        },
        async Api_postData(context, payload) {
            const { data } = await http.post("/", payload);
            context.commit('setTodoData', data);
        },
        async Api_EmptyTodoData(context, payload) {
            const { data } = await http.delete("/empty");
            context.commit('setTodoData', data);
        }
    },
    modules: {
    }
})
