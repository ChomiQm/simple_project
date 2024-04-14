<script setup>
import { ref, watch, defineProps, defineEmits } from 'vue';
import DocumentService from "@/services/DocumentService.js";

const props = defineProps({
  documentId: Number,
  modelValue: Object,
  visible: Boolean
});

const emit = defineEmits(['update:modelValue', 'close', 'update:visible']);
const editedDocument = ref({ ...props.modelValue });
const selectRef = ref(null);

const updateDocument = async () => {
  if (!editedDocument.value || !editedDocument.value.id) {
    console.error('Document ID is undefined or the document data is missing.');
    return;
  }

  const documentToUpdate = {
    ...editedDocument.value,
    id: parseInt(editedDocument.value.id),
  };

  try {
    const response = await DocumentService.updateDocument(documentToUpdate.id, documentToUpdate);
    emit('update', response.data);
    closeModal();
    window.location.reload();
    window.alert("Document updated");
  } catch (error) {
    console.error('Error updating document:', error.response.data);
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

watch(() => props.modelValue, (newVal) => {
  if (newVal && newVal.date) {
    const dateObj = new Date(newVal.date);
    editedDocument.value = {
      ...newVal,
      date: dateObj.toISOString().split('T')[0]
    };
  }
}, { immediate: true, deep: true });


const closeModal = () => {
  emit('close');
};
</script>


<template>
  <div class="update-document-modal" v-show="visible">
    <div class="modal-overlay" @click="closeModal"></div>
    <div class="modal-content">
      <h3>Update document</h3>
      <form @submit.prevent="updateDocument" class="form-layout">
        <div class="form-group">
          <label for="type">Type</label>
          <select
            ref="selectRef"
            @focus="styleDropdown"
            @blur="resetDropdownStyle"
            v-model="editedDocument.type"
            required
          >
            <option disabled value="">Select type</option>
            <option value="Invoice">Invoice</option>
            <option value="Order">Order</option>
            <option value="Receipt">Receipt</option>
          </select>
        </div>
        <div class="form-group">
          <label for="date">Date</label>
          <input id="date" type="date" v-model="editedDocument.date" required>
        </div>
        <div class="form-group">
          <label for="firstName">First name</label>
          <input id="firstName" type="text" v-model="editedDocument.firstName" required>
        </div>
        <div class="form-group">
          <label for="lastName">Last name</label>
          <input id="lastName" type="text" v-model="editedDocument.lastName" required>
        </div>
        <div class="form-group">
          <label for="city">City</label>
          <input id="city" type="text" v-model="editedDocument.city" required>
        </div>
        <div class="form-actions">
          <button type="submit" class="save-button">Update</button>
          <button type="button" @click="closeModal" class="cancel-button">Cancel</button>
        </div>
      </form>
    </div>
  </div>
</template>


<style scoped>
.update-document-modal {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
  background-color: rgba(0, 0, 0, 0.5);
  backdrop-filter: blur(5px);
}

.modal-overlay {
  position: absolute;
  width: 100%;
  height: 100%;
  cursor: default;
}

.modal-content {
  background: linear-gradient(145deg, #36013F, #3F3F3F);
  padding: 20px;
  border-radius: 5px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.3);
  min-width: 300px;
  z-index: 2;
  color: white;
  display: flex;
  flex-direction: column;
  align-items: stretch;
}

.form-layout {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.form-group {
  display: flex;
  flex-direction: column;
  width: 100%;
}

.form-group label {
  color: #ccc;
  margin-bottom: 5px;
}

.form-group input[type="text"],
.form-group input[type="date"],
.form-group select {
  padding: 10px;
  margin-top: 5px;
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.2);
  color: white;
  font-size: 16px;
  border-radius: 4px;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
  margin-top: 20px;
}

.save-button,
.cancel-button {
  padding: 10px 20px;
  border: none;
  font-size: 1rem;
  border-radius: 4px;
  cursor: pointer;
  transition: background 0.3s ease;
}

.save-button {
  background: linear-gradient(to right, #4CAF50, #151515);
  color: white;
}

.cancel-button {
  background: linear-gradient(to right, #F44336, #151515);
  color: white;
}

.save-button:hover,
.cancel-button:hover {
  opacity: 0.8;
}
</style>
