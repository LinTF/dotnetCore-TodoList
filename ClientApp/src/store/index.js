import { createStore } from 'vuex'
import { http } from '@/utils/http'

export default createStore({
    state: {
        isOK: false
    },
    getters: {
    },
    mutations: {
        setTestText(state, payload) {
        // state.testText = payload;
        }
    },
    actions: {
        async testGetApi() {
            // const test = http;
            // state.isOK = true;
            // if (test !== null) {
            //     state.isOK = true;
            // }

            const { data } = await http.get("/");
            return data;
        }
    },
    modules: {
    }
})
