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
            for (let todoGroup of payload) {
                todoGroup.isEdit = false;
            }

            state.todoData = payload;
        }
    },
    actions: {
        async Api_GetTodoData(context) {
            const { data } = await http.get("/TodoItems");
            context.commit('setTodoData', data);
        },
        async Api_PostTodoData(context, payload) {
            await http.post("/TodoItems", payload);
        },
        async Api_EmptyTodoData(context) {
            const { data } = await http.delete("/empty");
            context.commit('setTodoData', data);
        },
        async Api_PutTodoData(context, payload) {
            const { data } = await http.put("/TodoDateGroup/" + payload.groupID + "/todoItem/" + payload.id, payload);
            context.commit('setTodoData', data);
        },
        async Api_DeleteTodoItem(context, { dateID, itemID }) {
            const { data } = await http.delete(dateID + "/TodoItemDetails/" + itemID);
            context.commit('setTodoData', data);
        },
        async Api_SortTodoItem(context, payload) {
            const { data } = await http.put("/Sort", payload);
            context.commit('setTodoData', data);
        }
    },
    modules: {
    }
})
