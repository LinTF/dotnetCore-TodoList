<template>
    <div>
        <button type="submit" class="btn btn-clear mb-3" @click="clearTodoList">清空全部待辦事項</button>
    </div>
    <div class="container">
        <div id="edit-block">
            <div class="row">
                <div class="col-xl-4 col-md-5">
                    <div class="row justify-content-md-center">
                        <div class="col-lg-auto">
                            <label for="todoItem" class=" col-form-label">待辦日期：</label>
                        </div>
                        <div class="col-lg-8 not-left-right-padding">
                            <div class="dropdown">
                                <input type="date" v-model="selectedDate" class="form-control">
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-xl-8 col-md-7">
                    <div class="row justify-content-md-center">
                        <div class="col-lg-auto">
                            <label for="todoItem" class=" col-form-label">待辦事項：</label>
                        </div>
                        <div class="col-lg-8 col-md-9 not-left-right-padding">
                            <input type="text" class="form-control" id="todoItem" placeholder="請輸入待辦事項" v-model="todoItemText">
                        </div>
                        <div class="col-md-auto">
                            <button type="submit" class="btn btn-primary mb-3" @click="addTodoItem">新增</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <hr />
    </div>
    <div class="container-fluid">
        <div class="row">
            <todo />
        </div>
    </div>
</template>
  
<script>
    import todo from "@/components/todo.vue"
    import { validDate } from '@/assets/js/validDate';
  
    export default {
        name: 'todoList',
        data() {
            return {
                todoItemText: '',
                selectedDate: '',
                validDate
            }
        },
        components: {
            todo
        },
        async created() {
            await this.api_getTodoData();
            this.selectedDate = this.getTodayDate;
            this.formatDelBtn();
        },
        methods: {
            async addTodoItem() {
                // *** todoItem 陣列結構 [
                //   {
                //     date: '2023/05/19',
                //     isEdit: false,
                //     item: [
                //       { text: '待辦事項', isFinish: false }
                //     ]
                //   }
                // ]

                const todoItemText = this.todoItemText.trim();
                const todoDate = this.selectedDate;

                // 判斷是否為日期
                const isValidDate = validDate(todoDate);
                if (isValidDate === false) {
                    alert("請輸入待辦日期");

                    // 預設撈取當日日期
                    this.selectedDate = this.getTodayDate;
                }

                // 判斷是否有輸入待辦事項文字
                if (todoItemText === '') {
                    alert("請輸入待辦事項");
                }

                if (isValidDate && todoItemText !== '') {
                    let todoItem = [];
                    todoItem.push({ itemText: todoItemText });

                    let todoItemGroup = { itemDate: todoDate, TodoItems: todoItem };
                    await this.api_postTodoData(todoItemGroup);
                    await this.api_getTodoData();
                }
                
                // 清空輸入框
                this.todoItemText = '';

                // 還原預設隱藏刪除按鈕
                this.formatDelBtn();
            },
            formatDelBtn() {
                // 預設刪除按鈕隱藏
                for (const item of this.get_todoData) {
                    if (item.isEdit) {
                        item.isEdit = false;
                    }
                }
            },
            async clearTodoList() {
                const result = confirm("您確定要清空全部待辦事項嗎？")
                if (result) {
                    await this.api_emptyTodoData();
                    await this.api_getTodoData();
                }
            },
            async api_getTodoData() {
                await this.$store.dispatch('Api_GetTodoData');
            },
            async api_postTodoData(todoItemData) {
                await this.$store.dispatch('Api_PostTodoData', todoItemData);
            },
            async api_emptyTodoData() {
                await this.$store.dispatch('Api_EmptyTodoData');
            }
        },
        computed: {
            get_todoData() {
                return this.$store.state.todoData;
            },
            getTodayDate() {
                // 日期元件
                const today = new Date();
                const year = today.getFullYear();
                const month = String(today.getMonth() + 1).padStart(2, '0');
                const day = String(today.getDate()).padStart(2, '0');
                return `${year}-${month}-${day}`;
            }
        }
    }
</script>
  
<style scoped lang="scss">

</style>