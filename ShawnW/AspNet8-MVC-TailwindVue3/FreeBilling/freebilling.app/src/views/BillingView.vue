<script setup>
  import { ref, reactive, onMounted } from "vue";
  import axios from "axios";
  import state from "../state";
  import { useRouter } from "vue-router";

  const bill = ref({});
  const employees = reactive([]);
  const customers = reactive([]);
  const message = ref("foo");

  const router = useRouter();

  onMounted(async () => {
    try {
      if (!state.token) {
        router.push("/login");
      }
      const employeesResult = await axios.get("/api/employees", {
        headers: {
          "authorization": `Bearer ${state.token}`
        }
      });
      employees.splice(0, employees.length, ...employeesResult.data);

      const customersResult = await axios.get("/api/customers", {
        headers: {
          "authorization": `Bearer ${state.token}`
        }
      });
      customers.splice(0, customers.length, ...customersResult.data);
    } catch (e) {
      message.value = e;
    }

    // if (result.status === 200) {
    //   employees.splice(0, employees.length, ...result.data);
    // }
  });

  async function saveBill() {
    // TODO: add validation
    try {
      const result = axios.post("/api/timebills", bill, {
        headers: {
          "authorization": `Bearer ${state.token}`
        }
      });
      router.push("/");
    } catch (e) {
      message.value = e;
    }
  }
</script>

<template>
  <div class="w-96 mx-auto bg-white p-2">
    <h1>Billing</h1>
    <div v-if="message">{{ message }}</div>
    <form novalidate @submit.prevent="saveBill">
      <label for="time">HoursWorked</label>
      <input type="text" name="time" id="time" v-model="bill.hoursWorked" />

      <label for="workPerformed">Work Performed</label>
      <textarea rows="4" name="workPerformed" id="workPerformed" v-model="bill.workPerformed"></textarea>

      <labl for="employee">Employee</labl>
      <select id="employee" name="employee" v-model="bill.employeeId">
        <option v-for="e in employees" :key="e.id" :value="e.id">{{ e.name }}</option>
      </select>

      <labl for="rate">Rate</labl>
      <input type="number" id="rate" v-model="bill.billingRate" />

      <label for="client">Client</label>
      <select id="client" name="client" v-model="bill.customerId">
        <option>Pick One...</option>
        <option v-for="c in customers" :key="c.id" :value="c.id">{{ c.companyName }}</option>
      </select>

      <div>
        <button type="submit" class="bg-green-800 hover:bg-green700 mr-2">Save</button>
        <button>Cancel</button>
      </div>
    </form>
    <pre>{{ bill }}</pre>
  </div>
</template>
