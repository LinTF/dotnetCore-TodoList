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
            await http.delete("TodoItems/empty");
        },
        async Api_PutTodoData(context, payload) {
            await http.put("/TodoDateGroup/" + payload.groupID + "/todoItem/" + payload.id, payload);
        },
        async Api_DeleteTodoItem(context, { groupID, itemID }) {
            await http.delete("TodoDateGroup/" + groupID + "/todoItem/" + itemID);
        },
        async Api_SortTodoItem(context, payload) {
            await http.put("TodoItems/Sort", payload);
        }
    },
    modules: {
    }
})
