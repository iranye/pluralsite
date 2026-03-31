<script setup>
  import { ref, reactive, computed, onMounted } from "vue";
  import { formatMoney } from "../formatters";
  import axios from "axios";
  import WaitCursor from "../components/WaitCursor.vue";

  const name = ref("Ira");
  const nancy = ref("Nancy Smith");

  const isBusy = ref(false);

  const timeBills = reactive([
  ]);

  onMounted(async () => {
    try {
      isBusy.value = true;
      const result = await axios("/api/customers/2/timebills");
      console.log("result.status: " + result.status);
      if (result.status === 200) {
        timeBills.splice(0, timeBills.length, ...result.data);

        var firstBill = timeBills[0].billingRate * timeBills[0].hours;
        console.log("firstBill: " + firstBill);
      }
    } catch {
      console.log("Failure encountered in onMounted method");
    } finally {
      setTimeout(() => isBusy.value = false, 2000);
    }
  });

  const total = computed(() => {
    return timeBills.map(b => b.billingRate * b.hours)
      .reduce((b, t) => t + b, 0);
  });

  function changeMe() {
    name.value += "+";
    console.log(name.value);
  }

  function newItem() {
    timeBills.push({
      hours: 8,
      billingRate: 85.00,
      date: "2023-09-09",
      workPerformed: "chauffer",
      employeeid: 2,
      customerid: 2
    });
    console.log(timeBills.length);
  }

  // 11.5 (SEE COMMENT IN WaitCursor.vue)
  function handleHidden() {
    console.log("Wait Cursor now hidden!");
  }
</script>

<template>
  <header>
    <h3>Our App</h3>
  </header>

  <main>
    <h1>Hello from Vue</h1>
    <WaitCursor :busy="isBusy" msg="Please wait..." @onHidden="handleHidden"></WaitCursor>
    
    <!--
    <div>{{ name }}</div>
    <button class="btn" v-on:click=changeMe()>Change The Name</button>
    <button class="btn" v-on:click=newItem>NewItem</button>
    <img src="@/imgs/nancy.jpg" v-bind:alt="nancy" v-bind:title="nancy" />
    -->

    <table style="border: 1px solid black;">
      <thead>
        <tr>
          <td>Hours</td>
          <td>Date</td>
          <td>Description</td>
        </tr>
      </thead>
      <tbody>
        <tr v-for="tb in timeBills" style="border: 1px solid black;">
          <td>{{ tb.hours}}</td>
          <td>{{ tb.date }}</td>
          <td>{{ tb.workPerformed }}</td>
        </tr>
      </tbody>
    </table>
    <div>Total: {{ formatMoney(total) }}</div>
    <!--<div>Total: {{ total }}</div>-->
    <pre>
v-bind:alt="nancy" can be shortened to: :alt="nancy"
colon for attributes
@ for events
&#123;&#123; &#125;&#125; for content
</pre>
  </main>
</template>

