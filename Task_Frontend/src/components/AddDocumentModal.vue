<script setup>
import {ref, defineProps, defineEmits, toRefs, watch} from 'vue';
import DocumentService from '@/services/DocumentService';

const props = defineProps({
  visible: Boolean,
  userHasData: Boolean,
  userData: Object
});

const { userData } = toRefs(props);

watch(userData, (newVal) => {
  if (newVal) {
    Object.assign(newDocument.value, newVal);
  }
}, { deep: true });


const emit = defineEmits(['close']);
const selectRef = ref(null);

const newDocument = ref({
  type: '',
  date: '',
  firstName: '',
  lastName: '',
  city: ''
});

// func closing and emit to parent
const closeModal = () => {
  emit('close');
};

const submitNewDocument = async () => {
  try {
    await DocumentService.postDocument(newDocument.value);
    closeModal();
    window.location.reload();
    window.alert("Document added");
  } catch (error) {
    console.error('Error during document creation:', error);
  }
};

const styleDropdown = () => {
  if (selectRef.value) {
    selectRef.value.style.backgroundColor = '#3F3F3F';
    selectRef.value.style.color = 'white';
  }
};

const resetDropdownStyle = () => {
  if (selectRef.value) {
    selectRef.value.style.backgroundColor = '';
    selectRef.value.style.color = '';
  }
};
</script>

<template>
  <div class="add-document-modal" v-if="props.visible">
    <div class="modal-overlay" @click="closeModal"></div>
    <div class="modal-content">
      <h3>Add new document</h3>
      <div v-if="userHasData === false">
        <p>You need to provide your user data to add a document.</p>
      </div>
      <form @submit.prevent="submitNewDocument" class="form-layout" v-else>
        <div class="form-group">
          <label for="type">Type</label>
          <select id="type" v-model="newDocument.type" ref="selectRef" @focus="styleDropdown" @blur="resetDropdownStyle" required>
            <option value="">Select type</option>
            <option value="Invoice">Invoice</option>
            <option value="Order">Order</option>
            <option value="Receipt">Receipt</option>
          </select>
        </div>
        <div class="form-group">
          <label for="date">Date</label>
          <input id="date" type="date" v-model="newDocument.date" required>
        </div>
        <div class="form-group">
          <label for="firstName">First name</label>
          <input id="firstName" type="text" v-model="newDocument.firstName" required>
        </div>
        <div class="form-group">
          <label for="lastName">Last name</label>
          <input id="lastName" type="text" v-model="newDocument.lastName" required>
        </div>
        <div class="form-group">
          <label for="city">City</label>
          <input id="city" type="text" v-model="newDocument.city" required>
        </div>
        <div class="form-actions">
          <button type="submit">Save</button>
          <button type="button" @click="closeModal">Cancel</button>
        </div>
      </form>
    </div>
  </div>
</template>


<style scoped>
.add-document-modal {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 10;
  background-color: rgba(0, 0, 0, 0.5);
  backdrop-filter: blur(5px);
}

.modal-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: transparent;
}

.modal-content {
  position: relative;
  background: linear-gradient(145deg, #36013F, #3F3F3F);
  padding: 40px;
  border-radius: 10px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  z-index: 11;
  width: 80%;
  max-width: 500px;
}

.form-group {
  margin-bottom: 20px;
  width: 100%;
}

.form-group select {
  width: 100%;
  padding: 10px;
  border-radius: 5px;
  border: none;
  appearance: none;
  color: white;
  font-size: 16px;
}

.form-group select:focus {
  outline: none;
  box-shadow: 0 0 0 2px rgba(255, 255, 255, 0.2);
}

.form-layout {
  display: flex;
  flex-direction: column;
  align-items: center;
  width: 100%;
}

.form-group label {
  display: block;
  margin-bottom: 5px;
  color: #ccc;
}

.form-group input[type="text"],
.form-group input[type="date"],
.form-group select {
  width: 80%;
  padding: 12px;
  border: 1px solid rgba(255, 255, 255, 0.3);
  background: rgba(255, 255, 255, 0.05);
  color: white;
  font-size: 16px;
}

.form-actions {
  text-align: right;
  padding-top: 10px;
   width: 100%;
}

.form-actions button {
  padding: 10px 15px;
  margin-left: 10px;
  border-radius: 5px;
  border: none;
  cursor: pointer;
  font-size: 16px;
  transition: background 0.3s ease;
}

.form-actions button[type="submit"] {
  background: linear-gradient(to right, #4CAF50, #151515);
  color: white;
}

.form-actions button[type="button"] {
  background: linear-gradient(to right, #F44336, #151515);
  color: white;
}

button:hover {
  opacity: 0.8;
}
</style>
