<template>
    <div class="col-xl-4 col-md-6 date-black" v-for="(todo, dateIndex) in propsTodo" :key="dateIndex">
        <h3>{{ todo.date }}
            <button class="del-icon" 
                @click="showDelBtn(todo.date)">
                <i class="fa-regular fa-trash-can"></i>
            </button>
        </h3>
        <div class="todo-block">
            <div>
                <draggable 
                    group="group"
                    :list="todo.item"
                    :item-key="item => item.text"
                    :force-fallback="true"
                    chosen-class="chosen"
                    animation="300"
                    @start="onStart"
                    @end="onEnd">

                    <template #item="{ element, index }">
                        <div 
                            class="row item"
                            :style="{ 'background-color': element.isFinish === false ? '#f8e4cc' : '#fff' }">
        
                            <div class="col-7 vertical-center">
                                <input type="checkbox" :id="todo.date+index"
                                        :name="todo.date+index" 
                                        @change="getCheckedItem(dateIndex, index)"
                                        :checked="element.isFinish"
                                        :disabled="element.isFinish">
                                <span>{{ index + 1 + ". &nbsp;" }}</span>
                                <label :for="todo.date+index" 
                                    :style="{ 'text-decoration': element.isFinish === true ? 'line-through' : 'none' }">
                                    {{ element.text }}
                                </label>
                            </div>
                            <div class="col-5 btn-block">
                                <button type="submit" class="btn btn-danger mb-3" 
                                :style="{ visibility: todo.isEdit === true ? 'unset' : 'hidden', 
                                        display: todo.isEdit === false && element.isFinish === true ? 'none' : 'block' }" 
                                @click="deleteTodoItem(todo.date, dateIndex, index, element.text)">刪除</button>
        
                                <button type="submit" class="btn return mb-3" 
                                :style="{ display: todo.isEdit === false && element.isFinish === true ? 'block' : 'none' }" 
                                @click="getCheckedItem(dateIndex, index)">取消勾選</button>
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
            }
        },
        components: {
            draggable
        },
        methods: {
            deleteTodoItem(date, dateIndex, itemIndex, itemText) {
                const checkDelete = confirm("您確定要刪除 " + date + '-' + itemText + " 嗎？");
                if (checkDelete) {
                    // 先找到要刪除的日期
                    const dateData = this.propsTodo.find(item => item.date === date);
                    // 計算該日期有幾筆資料
                    const dataCount = dateData.item.length;

                    // 如果資料量大於一筆，則刪除該項目，其他．則將該日期資料全刪除
                    if (dataCount > 1) {
                        dateData.item.splice(itemIndex, 1);
                    } else {
                        this.propsTodo.splice(dateIndex, 1);
                    }

                    // 切完之後畫面的刪除按鈕隱藏
                    this.showDelBtn(date)
                    
                    // 將 localStorage 陣列裝回
                    localStorage.setItem('todoItem', JSON.stringify(this.propsTodo));
                }
            },
            showDelBtn(dateVal) {
                for (const todo of this.propsTodo) {
                    if (todo.date === dateVal) {
                        todo.isEdit = !todo.isEdit;
                    } else {
                        todo.isEdit = false;
                    }
                }
            },
            getCheckedItem(dateIndex, itemIndex) {
                this.propsTodo[dateIndex].item[itemIndex].isFinish = !this.propsTodo[dateIndex].item[itemIndex].isFinish

                // 將 localStorage 陣列裝回
                localStorage.setItem('todoItem', JSON.stringify(this.propsTodo));
            },
            onStart() {},
            onEnd() {
                // 將 localStorage 陣列裝回
                localStorage.setItem('todoItem', JSON.stringify(this.propsTodo));
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