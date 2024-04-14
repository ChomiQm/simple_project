<script setup>
import {defineProps, defineEmits, ref, onMounted, watch} from 'vue';
import DocumentItemService from '@/services/DocumentItemService';

const props = defineProps({
  visible: Boolean,
  documentItems: Array,
  documentId: {
    type: Number,
    required: true
  }
});

const emit = defineEmits(['close', 'item-added']);
const selectedProduct = ref('');
let availableItems = ref([]);
const editingItem = ref(null);

const fetchAvailableItems = async () => {
  try {
    const response = await DocumentItemService.getAllDocumentItems();
    availableItems.value = response.data;
  }
  catch (error){
    console.error('Error fetching available items, items not found. ', error);
  }
};

// Referencje do nowych danych elementu
const newItem = ref({
  product: '',
  quantity: 1,
  price: 0.01,
  taxRate: 0
});

const submitNewItem = async () => {
  try {
     if (!props.documentId) {
    console.error('Document ID is not provided.');
    return;
  }
     const itemToAdd = {
       ...newItem.value,
    documentId: props.documentId,
  };

    const response = await DocumentItemService.postDocumentItemToDocument(props.documentId, itemToAdd);
    emit('item-added', response.data);
    newItem.value = { product: '', quantity: 1, price: 0.01, taxRate: 0 };
    emit('close'); // Zamknięcie modala
  } catch (error) {
    console.error('Error adding new document item:', error);
  }
};

const close = () => {
  emit('close');
};

watch(selectedProduct, (newProductName) => {
  if (newProductName) {
    const foundItem = availableItems.value.find(item => item.product === newProductName);
    if (foundItem) {
      newItem.value = {
        product: foundItem.product,
        quantity: foundItem.quantity,
        price: foundItem.price,
        taxRate: foundItem.taxRate
      };
    }
  } else {
    newItem.value = {
      product: '',
      quantity: 1,
      price: 0.01,
      taxRate: 0
    };
  }
});

const beginEdit = (item) => {
  editingItem.value = { ...item };
};

const cancelEdit = () => {
  editingItem.value = null;
};

const saveEdit = async () => {
  try {
    await DocumentItemService.updateDocumentItem(editingItem.value.documentId, editingItem.value.ordinal, editingItem.value);
    emit('item-updated', editingItem.value);
    editingItem.value = null;
    window.location.reload();
    window.alert("Changes saved");
  } catch (error) {
    console.error('Failed to save item:', error);
    alert(`Failed to save item with ordinal ${editingItem.value.ordinal}.`);
  }
};

const deleteItem = async (ordinal) => {
  try {
    await DocumentItemService.deleteDocumentItem(props.documentId, ordinal);
    emit('item-removed');
    window.location.reload();
    window.alert('Element deleted');
  } catch (error) {
    console.error('Failed to delete item:', error);
    alert(`Failed to delete item with ordinal ${ordinal}.`);
  }
};

onMounted(fetchAvailableItems);

</script>

<template>
  <div class="modal" v-if="props.visible">
    <div class="modal-content">
      <h3>Document Items</h3>
      <div class="items-list">
        <div v-for="item in props.documentItems" :key="item.ordinal" class="item-row">
           <div class="edit-form" v-if="editingItem && editingItem.ordinal === item.ordinal">
              <div class="input-group">
                <label>Product:</label>
                <input type="text" v-model="editingItem.product">
              </div>
              <div class="input-group">
                <label>Quantity:</label>
                <input type="number" v-model.number="editingItem.quantity">
              </div>
              <div class="input-group">
                <label>Price:</label>
                <input type="number" v-model.number="editingItem.price">
              </div>
              <div class="input-group">
                <label>Tax Rate:</label>
                <select v-model.number="editingItem.taxRate">
                  <option value="8">8%</option>
                  <option value="23">23%</option>
                </select>
              </div>
              <div class="form-actions">
                <button class="save-button" @click="saveEdit">Save</button>
                <button class="cancel-button" @click="cancelEdit">Cancel</button>
              </div>
           </div>
          <div v-else class="item-details">
            {{ item.product }} - Quantity: {{ item.quantity }} - Price: {{ item.price.toFixed(2) }} - Tax: {{ item.taxRate }}%
          </div>
          <div class="item-actions">
            <button v-if="!editingItem || editingItem.ordinal !== item.ordinal" class="update-button" @click="beginEdit(item)">Update</button>
            <button class="delete-button" @click="deleteItem(item.ordinal)">Delete</button>
          </div>
        </div>
      </div>
      <div class="add-item-form">
        <h3>Add New Item</h3>
        <form @submit.prevent="submitNewItem">
          <div class="form-group">
            <label for="product-select">Product:</label>
            <select id="product-select" v-model="selectedProduct" required>
              <option disabled value="">Select a product</option>
              <option v-for="item in availableItems" :value="item.product" :key="item.ordinal">
                {{ item.product }}
              </option>
            </select>
          </div>
          <div class="form-group">
            <label for="quantity">Quantity:</label>
            <input id="quantity" v-model.number="newItem.quantity" type="number" placeholder="Quantity" min="1" />
          </div>
          <div class="form-group">
            <label for="price">Price:</label>
            <input id="price" v-model.number="newItem.price" type="number" placeholder="Price" min="0.01" step="0.01" />
          </div>
          <div class="form-group">
            <label for="taxRate">Tax Rate:</label>
            <select id="taxRate" v-model.number="newItem.taxRate" required>
              <option value="8">8%</option>
              <option value="23">23%</option>
            </select>
          </div>
          <div class="form-actions">
            <button type="submit">Add Item</button>
            <button type="button" @click="close">Close</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>


<style scoped>
.modal {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  backdrop-filter: blur(5px);
}

.modal-content {
  background: linear-gradient(145deg, #36013F, #3F3F3F);
  padding: 20px;
  border-radius: 5px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.3);
  min-width: 300px;
  color: white;
}

.items-list {
  margin-bottom: 20px;
  max-height: 300px;
  overflow-y: auto;
}

.item-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 10px;
  padding: 10px;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 10px;
}

.item-details,
.edit-form {
  flex-grow: 1;
  display: flex;
  flex-direction: column;
  margin-right: 8px;
}

.input-group {
  display: flex;
  flex-direction: column;
  margin-bottom: 10px;
}

.input-group label {
  font-size: 0.9em;
  color: #ccc;
}

.input-group input,
.input-group select {
  padding: 10px;
  border-radius: 4px;
  border: 1px solid #ddd;
  margin-top: 5px;
}

.item-actions {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
}

.item-actions button {
  margin-bottom: 5px;
}

.form-actions {
  display: flex;
  justify-content: center;
  align-items: center;
  margin-top: 10px;
}

.save-button,
.cancel-button,
.delete-button {
  padding: 6px 12px;
  border-radius: 4px;
  cursor: pointer;
  margin: 0 2px; /* Spacing between buttons */
}

.save-button {
  background: linear-gradient(to right, #4CAF50, #151515);
  color: white;
}

.cancel-button {
  background: linear-gradient(to right, #F44336, #151515);
  color: white;
}

.delete-button {
  background: linear-gradient(to right, #530000, #151515);
  color: white;
  border: none;
  padding: 6px 12px;
  border-radius: 4px;
  cursor: pointer;
  transition: background 0.3s ease;
}

.update-button {
  background: linear-gradient(to right, #554904, #151515);
  color: white;
  border: none;
  padding: 6px 12px;
  border-radius: 4px;
  cursor: pointer;
  transition: background 0.3s ease;
}

.delete-button:hover {
  background: linear-gradient(to right, #730000, #191919);
}
.update-button:hover {
  background: linear-gradient(to right, #957F07, #191919);
}


button:hover {
  opacity: 0.8;
}
</style>


