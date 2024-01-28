<template>
    <div class="col-xl-4 col-md-6 date-black" :data-id="todo.id" v-for="(todo, dateIndex) in get_todoData" :key="dateIndex">
        <h3>{{ formatDate(todo.itemDate) }}
            <button class="del-icon" 
                @click="showDelBtn(todo.itemDate)">
                <i class="fa-regular fa-trash-can"></i>
            </button>
        </h3>
        <!-- <p>{{ todo.isEdit }}</p> -->
        <div class="todo-block">
            <div>
                <draggable 
                    group="group"
                    :list="todo.todoItems"
                    :item-key="item => item.id"
                    :force-fallback="false"
                    chosen-class="chosen"
                    animation="300"
                    @start="onStart"
                    @end="onEnd">

                    <template #item="{ element, index }">
                        <div class="row item"
                            :style="{ 'background-color': element.isFinish === false ? '#f8e4cc' : '#fff' }">
        
                            <div class="col-7 vertical-center">
                                <input type="checkbox" :id="todo.itemDate+index"
                                        :name="todo.itemDate+index" 
                                        @change="getCheckedItem(element)"
                                        :checked="element.isFinish"
                                        :disabled="element.isFinish">
                                <span>{{ index + 1 + ". &nbsp;" }}</span>
                                <label :for="todo.itemDate+index" 
                                    :style="{ 'text-decoration': element.isFinish === true ? 'line-through' : 'none' }">
                                    {{ element.itemText }}
                                </label>
                            </div>
                            <div class="col-5 btn-block">
                                <button type="submit" class="btn btn-danger mb-3" 
                                :style="{ visibility: todo.isEdit === true ? 'unset' : 'hidden', 
                                        display: todo.isEdit === false && element.isFinish === true ? 'none' : 'block' }" 
                                @click="deleteTodoItem(todo.id, element.id, todo.itemDate, element.itemText)">刪除</button>
        
                                <button type="submit" class="btn return mb-3" 
                                :style="{ display: todo.isEdit === false && element.isFinish === true ? 'block' : 'none' }" 
                                @click="getCheckedItem(element)">取消勾選</button>
                            </div>
                        </div>
                    </template>
                </draggable>
            </div>
        </div>
    </div>
</template>

<script>
    import draggable from 'vuedraggable'
    import { formatDate } from '@/assets/js/formatDate';

    export default {
        name: 'todo',
        props: {
            propsTodo: {
                type: Array,
                required: true
            }
        },
        data() {
            return {
                // public js
                formatDate
            }
        },
        emits: ['updatePropsTodo'],
        components: {
            draggable
        },
        methods: {
            async deleteTodoItem(groupID, itemID, date, itemText) {
                const checkDelete = confirm("您確定要刪除 " + formatDate(date) + '：' + itemText + " 嗎？");
                if (checkDelete) {
                    await this.api_deleteItem(groupID, itemID);
                    await this.getApi_todoData();
                }
            },
            showDelBtn(dateVal) {
                for (const todo of this.get_todoData) {
                    if (todo.itemDate === dateVal) {
                        todo.isEdit = !todo.isEdit;
                    } else {
                        todo.isEdit = false;
                    }
                }
            },
            getCheckedItem(itemData) {
                // 將資料使用 api 回傳至後端
                itemData.isFinish = !itemData.isFinish
                this.putApi_todoData(itemData)
            },
            onStart() {
            },
            async onEnd(e) {
                const newTodoList = JSON.parse(JSON.stringify(this.get_todoData));
                const fromTodoID = e.from.closest('[data-id]').dataset.id;
                const toTodoID = e.to.closest('[data-id]').dataset.id;
                const fromData = newTodoList.find(todo => todo.id === Number(fromTodoID));
                const toData = newTodoList.find(todo => todo.id === Number(toTodoID));
                const onlyFromItemData = fromData.todoItems;
                const onlyToItmeData = toData.todoItems;

                let putTodoItemData = [];

                console.log("putTodoItemData:" + JSON.stringify(putTodoItemData));
                console.log("onlyToItmeData:" + JSON.stringify(onlyToItmeData));

                if (fromTodoID !== toTodoID) {
                    for (let i = 0; i < onlyFromItemData.length; i++) {
                        // let getTodoItemIdCol = onlyFromItemData[i].todoItemId
                        // getTodoItemIdCol = fromTodoID;
                        let getTodoItemIdCol = fromTodoID
                        putTodoItemData.push({ id: onlyFromItemData[i].id, todoGroupId: getTodoItemIdCol })
                    }
                }

                for (let i = 0; i < onlyToItmeData.length; i++) {
                    // let getTodoItemIdCol = onlyToItmeData[i].todoItemId
                    // getTodoItemIdCol = toTodoID;
                    let getTodoItemIdCol = toTodoID;
                    putTodoItemData.push({ id: onlyToItmeData[i].id, todoGroupId: getTodoItemIdCol })
                }

                await this.api_PutTodoItemSort(putTodoItemData);
                await this.getApi_todoData();
            },
            async putApi_todoData(itemData) {
                await this.$store.dispatch('Api_PutTodoData', itemData);
            },
            async api_deleteItem(groupID, itemID) {
                await this.$store.dispatch('Api_DeleteTodoItem', { groupID, itemID });
            },
            async api_PutTodoItemSort(detailData) {
                await this.$store.dispatch('Api_SortTodoItem', detailData);
            },
            async getApi_todoData() {
                await this.$store.dispatch('Api_GetTodoData');
            }
        },
        computed: {
            get_todoData() {
                return this.$store.state.todoData;
            }
        }
    }
</script>

<style scoped lang="scss">
    .todo-block {
        padding: 0px 10px 10px 10px;
        border-radius: 10px;
        background-color: #fff;

        .item {
            border: 1px solid #f8e4cc;

            &:hover{
                border: 1px solid #d6279c;
            }
        }

        .return {
            color: #d4ac7c;
            font-size: 14px;
        }

        .btn-danger {
            font-size: 14px;
        }
    }
    
    .date-black {
        margin-bottom: 30px;
    }
</style>