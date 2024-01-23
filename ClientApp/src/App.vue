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
        <!-- <div>
            <button type="submit" class="btn btn-primary mb-3" @click="apiTest">API</button>
            <p>{{ getTestText }}</p>
        </div> -->
        <hr />
    </div>
    <div class="container-fluid">
        <div class="row">
            <!-- <todo :propsTodo="todoItemSort" /> -->
            <todo :propsTodo="this.todoItem" @updatePropsTodo="getApi_todoData" />
        </div>
    </div>
</template>
  
<script>
    import todo from "@/components/todo.vue"
    import axios from 'axios'
    // import { Action } from "vuex"
  
    export default {
        name: 'todoList',
        data() {
            return {
                todoItem: [],
                todoItemText: '',
                selectedDate: ''
            }
        },
        components: {
            todo
        },
        async created() {
            // // 當元件被建立時讀取 localstorage 資料
            // const todoListData = localStorage.getItem("todoItem");
            // if (todoListData) {
            //   this.todoItem = JSON.parse(todoListData);
            // }

            await this.getApi_todoData();
    
            // 日期元件
            const today = new Date();
            const year = today.getFullYear();
            const month = String(today.getMonth() + 1).padStart(2, '0');
            const day = String(today.getDate()).padStart(2, '0');
            this.selectedDate = `${year}-${month}-${day}`;
    
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
                const isValidDate = this.isValidDate(todoDate);

                // 判斷是否有輸入待辦事項文字
                if (todoItemText === '') {
                    alert("請輸入待辦事項");
                }

                if (isValidDate && todoItemText !== '') {
                    let todoItem = [];
                    todoItem.push({ itemText: todoItemText });

                    let todoItemGroup = { itemDate: todoDate, TodoItems: todoItem };
                    await this.api_postData(todoItemGroup);
                }

                // 清空輸入框
                this.todoItemText = '';

                // 還原預設隱藏刪除按鈕
                this.formatDelBtn();
            },
            formatDate(dateStr) {
                const [year, month, day] = dateStr.split('-');
    
                // 格式化日期
                return `${year}/${month}/${day}`;
            },
            isValidDate(dateString) {
                // 將正確日期格式拆開
                const [year, month, day] = dateString.split('-');
        
                // 檢查年份是否不小於 1911
                if (year < 1911) {
                    return false;
                }
        
                // 檢查月份是否在 1 到 12 的範圍內
                if (month < 1 || month > 12) {
                    return false;
                }
    
                // 檢查日期是否在合法範圍內
                if (day < 1 || day > 31) {
                    return false;
                }
        
                return true;
            },
            formatDelBtn() {
                // 預設刪除按鈕隱藏
                for (const item of this.todoItem) {
                    if (item.isEdit) {
                    item.isEdit = false;
                    }
                }
            },
            async clearTodoList() {
                const result = confirm("您確定要清空全部待辦事項嗎？")
                if (result) {
                    // this.todoItem = [];
                    // localStorage.clear();


                    
                    // this.api_emptyData()
                    //   .then(() => {
                    //     // 再次取得資料
                    //     this.getApi_todoData();
                    //   });

                    await this.$store.dispatch('Api_EmptyTodoData');
            }
            },
            async apiTest() {
                const aaa = await axios.get("https://localhost:7268/api/TodoItems")
                // const aaa = await axios.get("https://localhost:7268/api/TodoItems/2")

                console.log(aaa.data);
            },
            async getApi_todoData() {
                // const todoListData = await axios.get("https://localhost:7268/api/TodoItems");
                // this.todoItem = todoListData.data;

                // this.todoItem = await this.$store.dispatch('Api_GetTodoData');
                await this.$store.dispatch('Api_GetTodoData');

                // console.log(JSON.stringify(this.$store.state.todoData));
                // console.log(this.todoItem);

                // for (const todo of this.todoItem ) {
                //   todo.isEdit = false;
                // }

                // for (const todo of this.$store.state.todoData ) {
                //   todo.isEdit = false;
                // }
            },
            async api_emptyData() {
                // const emptyApi = await axios.delete("https://localhost:7268/api/TodoItems/empty");
                // emptyApi.data;

                await axios.delete("https://localhost:7268/api/TodoItems/empty");
            },
            async api_postData(todoItemData) {
                await this.$store.dispatch('Api_PostTodoData', todoItemData);
            },
            async testMutations() {
                // this.$store.commit('setTestText', 'is test Mutations');
                // console.log(this.$store.state.isOk);
                console.log(await this.$store.dispatch('testGetApi'));
            }
        },
        computed: {
            // 待辦：排序從後端做，連資料庫之後改
            todoItemSort() {
                return this.todoItem.sort((a, b) => {
                    const dateA = new Date(a.itemDate);
                    const dateB = new Date(b.itemDate);

                    return dateB - dateA;
                });
            },
            getTestText() {
                return "";
            }
        }
    }
</script>
  
<style scoped lang="scss">
    #edit-block {
      margin-top: 20px;
    }
  
    .dropdown {
      text-align: center;
    }
  
    hr {
      border: 1px solid #ccc;
      margin: 10px auto 30px auto;
    }
  
    .btn-primary  {
      display: block;
      margin: 0px auto 0px auto;
    }
  
    .btn-clear {
      border: 1px solid #d4ac7c;
      color: #d4ac7c;
      display: block;
      margin: 20px 10px 0px auto;
      font-size: 12px;
    }
  
    @media screen and (max-width: 767px) {
      .btn-primary {
        margin-top: 10px;
      }
    }
  
    @media screen and (min-width: 768px) {
      .not-left-right-padding {
        padding-left: 0px;
        padding-right: 0px;
      }
    }
  
    .testa {
      background-color: #d4ac7c;
    }
    .testb {
      background-color: #ccc;
    }
</style>