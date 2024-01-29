<template>
    <div class="col-xl-4 col-md-6 date-black" :data-id="todo.id" v-for="(todo, dateIndex) in get_todoData" :key="dateIndex">
        <h3>{{ formatDate(todo.itemDate) }}
            <button class="del-icon" 
                @click="showDelBtn(todo.itemDate)">
                <i class="fa-regular fa-trash-can"></i>
            </button>
        </h3>
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
        data() {
            return {
                // public js
                formatDate
            }
        },
        components: {
            draggable
        },
        methods: {
            showDelBtn(groupDate) {
                for (const todo of this.get_todoData) {
                    if (todo.itemDate === groupDate) {
                        todo.isEdit = !todo.isEdit;
                    } else {
                        todo.isEdit = false;
                    }
                }
            },
            async deleteTodoItem(groupID, itemID, date, itemText) {
                const checkDelete = confirm("您確定要刪除 " + formatDate(date) + '：' + itemText + " 嗎？");
                if (checkDelete) {
                    await this.api_deleteTodoItem(groupID, itemID);
                    await this.api_getTodoData();
                }
            },
            getCheckedItem(itemData) {
                itemData.isFinish = !itemData.isFinish
                this.api_putTodoData(itemData)
            },
            onStart() {
            },
            async onEnd(e) {
                const getTodoList = JSON.parse(JSON.stringify(this.get_todoData));
                const fromGroupID = e.from.closest('[data-id]').dataset.id;
                const toGroupID = e.to.closest('[data-id]').dataset.id;
                const fromData = getTodoList.find(todo => todo.id === Number(fromGroupID));
                const toData = getTodoList.find(todo => todo.id === Number(toGroupID));
                const fromTodoItems = fromData.todoItems;
                const toTodoItems = toData.todoItems;

                let newItemList = [];
                if (fromGroupID !== toGroupID) {
                    for (let i = 0; i < fromTodoItems.length; i++) {
                        newItemList.push({ id: fromTodoItems[i].id, todoGroupId: fromGroupID })
                    }
                }

                for (let i = 0; i < toTodoItems.length; i++) {
                    newItemList.push({ id: toTodoItems[i].id, todoGroupId: toGroupID })
                }

                await this.api_sortTodoItem(newItemList);
                await this.api_getTodoData();
            },
            async api_putTodoData(itemData) {
                await this.$store.dispatch('Api_PutTodoData', itemData);
            },
            async api_deleteTodoItem(groupID, itemID) {
                await this.$store.dispatch('Api_DeleteTodoItem', { groupID, itemID });
            },
            async api_sortTodoItem(detailData) {
                await this.$store.dispatch('Api_SortTodoItem', detailData);
            },
            async api_getTodoData() {
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
    
</style>