<script setup>

  //defineProps(["busy", "msg"])
  defineProps({
    "busy": { required: true, type: Boolean },
    "msg": { type: String, default: "Loading..."} //msg is optional for the parent component
  });

  // 11.5 SW implements then comments out (never got this working)
  const emits = defineEmits(["onHidden"]);

  function hide() {
    emits.emit("onHidden", "hello");
  }

</script>
<template>
  <div v-if="busy" class="bg-amber-700 text-white p-2 text-lg">{{ msg }}</div>

</template>
