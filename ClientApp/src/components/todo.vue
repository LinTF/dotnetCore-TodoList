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
                                        @change="getCheckedItem(todo.id ,element)"
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
                                @click="getCheckedItem(todo.id ,element)">取消勾選</button>
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
    import axios from 'axios'

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
            async deleteTodoItem(dateID, itemID, date, itemText) {
                const checkDelete = confirm("您確定要刪除 " + formatDate(date) + '：' + itemText + " 嗎？");
                if (checkDelete) {
                    // // 先找到要刪除的日期
                    // const dateData = this.propsTodo.find(item => item.date === date);
                    // // 計算該日期有幾筆資料
                    // const dataCount = dateData.item.length;

                    // // 如果資料量大於一筆，則刪除該項目，其他．則將該日期資料全刪除
                    // if (dataCount > 1) {
                    //     dateData.item.splice(itemIndex, 1);
                    // } else {
                    //     this.propsTodo.splice(dateIndex, 1);
                    // }

                    // // 切完之後畫面的刪除按鈕隱藏
                    // this.showDelBtn(date)
                    
                    // // 將 localStorage 陣列裝回
                    // localStorage.setItem('todoItem', JSON.stringify(this.propsTodo));

                    // this.api_deleteItem(dateID, itemID)
                    //     .then(() => {
                    //         this.$emit('updatePropsTodo');
                    //     });

                    // await this.api_deleteItem(dateID, itemID);
                    // this.$emit('updatePropsTodo');
                    
                    await this.$store.dispatch('Api_DeleteTodoItem', { dateID, itemID });
                }
            },
            showDelBtn(dateVal) {
                // for (const todo of this.propsTodo) {
                //     if (todo.itemDate === dateVal) {
                //         todo.isEdit = !todo.isEdit;
                //     } else {
                //         todo.isEdit = false;
                //     }
                // }


                for (const todo of this.get_todoData) {
                    if (todo.itemDate === dateVal) {
                        todo.isEdit = !todo.isEdit;
                    } else {
                        todo.isEdit = false;
                    }
                }
            },
            getCheckedItem(id, data) {
                // 將資料使用 api 回傳至後端
                data.isFinish = !data.isFinish
                this.putApi_todoData(id, data)
            },
            onStart() {
            },
            onEnd(e) {
                const newTodoList = JSON.parse(JSON.stringify(this.get_todoData));
                const fromTodoID = e.from.closest('[data-id]').dataset.id;
                const toTodoID = e.to.closest('[data-id]').dataset.id;
                const fromData = newTodoList.find(todo => todo.id === Number(fromTodoID));
                const toData = newTodoList.find(todo => todo.id === Number(toTodoID));
                const onlyFromItemData = fromData.todoItems;
                const onlyToItmeData = toData.todoItems;

                let putTodoItemData = [];

                if (fromTodoID !== toTodoID) {
                    for (let i = 0; i < onlyFromItemData.length; i++) {
                        let getTodoItemIdCol = onlyFromItemData[i].todoItemId
                        getTodoItemIdCol = fromTodoID;
                        putTodoItemData.push({ id: onlyFromItemData[i].id, todoItemId: getTodoItemIdCol })
                    }
                }

                for (let i = 0; i < onlyToItmeData.length; i++) {
                    let getTodoItemIdCol = onlyToItmeData[i].todoItemId
                    getTodoItemIdCol = toTodoID;
                    putTodoItemData.push({ id: onlyToItmeData[i].id, todoItemId: getTodoItemIdCol })
                }

                // api
                // this.api_PutTodoItemSort(putTodoItemData)
                //     .then(() => {
                //         this.$emit('updatePropsTodo');
                //     });

                this.api_PutTodoItemSort(putTodoItemData)
            },
            async putApi_todoData(id, data) {
                // const url = "https://localhost:7268/api/TodoItems/" + id + "/TodoItemDetails/" + data.id;
                // await axios.put(url, data)

                await this.$store.dispatch('Api_PutTodoData', data);
            },
            async api_deleteItem(dateID, itemID) {
                await axios.delete("https://localhost:7268/api/TodoItems/"+ dateID + "/TodoItemDetails/" + itemID);
            },
            async api_PutTodoItemSort(detailData) {
                // const url = "https://localhost:7268/api/TodoItems/Sort"
                // await axios.put(url, detailData);

                await this.$store.dispatch('Api_SortTodoItem', detailData);
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