<script setup>
import {ref, onMounted, reactive} from 'vue';
import DocumentService from '@/services/DocumentService.js';
import Pagination from '@/components/Pagination.vue';
import DocumentItemService from '@/services/DocumentItemService.js';
import DetailsModal from "@/components/DetailsModal.vue";
import AddDocumentModal from "@/components/AddDocumentModal.vue";
import UpdateDocumentModal from "@/components/UpdateDocumentModal.vue";
import UserDataService from "@/services/UserDataService.js";
import userDataService from "@/services/UserDataService.js";

const documents = ref([]);
const totalItems = ref(0);
const totalPages = ref(0);
const pageIndex = ref(0);
const pageSize = ref(10);
const search = ref('');
const sortBy = ref('');
const sortDescending = ref(false);

const detailsModalVisible = ref(false);
const addDocumentModalVisible = ref(false);
const documentItems = ref([]);
const currentDocumentId = ref(null);
const updateDocumentModalVisible = ref(false);
const selectedDocument = ref({});
let userHasData = ref(null);

const userData = reactive({
  firstName: '',
  lastName: '',
  city: ''
});

const fetchDocuments = () => {
  DocumentService.getDocuments(pageIndex.value, pageSize.value, search.value, sortBy.value, sortDescending.value)
    .then(response => {
      documents.value = response.data.items;
      totalItems.value = response.data.totalItems;
      totalPages.value = response.data.totalPages;
    })
    .catch(error => {
      console.error('Error fetching documents:', error);
    });
};

const formatDate = (date) => new Date(date).toLocaleDateString();

const handleChangePage = (newPage) => {
  pageIndex.value = newPage;
  fetchDocuments();
};

const state = reactive({
  userHasData: false,
  isDataLoaded: false // dodatkowy stan do kontrolowania, kiedy dane są załadowane
});

const showDetails = async (documentId) => {
  currentDocumentId.value = documentId;
  try {
    const response = await DocumentItemService.getDocumentItemsByDocumentId(documentId);
    documentItems.value = response.data;
    detailsModalVisible.value = true;
  } catch (error) {
    console.error('Error fetching document items:', error);
  }
};

const openUpdateDocumentModal = (document) => {
  console.log(document);
  selectedDocument.value = { ...document };
  updateDocumentModalVisible.value = true;
};

const handleUpdate = async updatedDocument => {
  try {
    await DocumentService.updateDocument(updatedDocument.id, updatedDocument);
    await fetchDocuments();
  } catch (error) {
    console.error('Error during document update:', error);
  }
  updateDocumentModalVisible.value = false;
};

const deleteDocument = async (id) => {
  if (window.confirm("Do you really want to delete that document?")) {
    try {
      await DocumentService.deleteDocument(id);
      await fetchDocuments();
      window.location.reload();
      window.alert("Document was deleted");
    } catch (error) {
      console.error('An error occured while deleting document:', error.response ? error.response.data : error);
      alert("Cannot delete document");
    }
  }
};

const deleteAllDocuments = async () => {
  if (window.confirm("Do you really wanna do it wtf?")) {
    try {
      const response = await DocumentService.deleteAllDocuments();
      window.location.reload();
      await fetchDocuments();
      window.alert(response.data.message);
    } catch (error) {
      console.error('Error:', error.response ? error.response.data : error);
      alert("Wystąpił błąd podczas usuwania dokumentów.");
    }
  }
};

const checkUserHasData = async () => {
  try {
    const response = await UserDataService.hasUserData();
    state.userHasData = response.data;
    state.isDataLoaded = true;
    if (state.userHasData) {
      await loadUserData();
    }
  } catch (error) {
    console.error('Error checking if user has data:', error);
  }
};
const loadUserData = async () => {
  try {
    const response = await userDataService.getActualUserData();
    Object.assign(userData, response.data);
  } catch (error) {
    console.error('Error loading user data:', error);
  }
};

const openAddDocumentModal = () => {
  addDocumentModalVisible.value = true;
};

onMounted(async () => {
  await fetchDocuments();
  await checkUserHasData();
});

</script>

<template>
  <div class="documents">
    <div class="search">
      <input type="text" v-model="search" placeholder="Search documents...">
      <button @click="fetchDocuments">Search!</button>
      <button @click="openAddDocumentModal">Add Document</button>
      <button @click="deleteAllDocuments">DROP DATABASE</button>
    </div>
    <div v-if="documents.length > 0">
      <table>
        <thead>
          <tr>
            <th>ID</th>
            <th>Type</th>
            <th>Date</th>
            <th>First Name</th>
            <th>Last Name</th>
            <th>City</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="document in documents" :key="document.id">
            <td>{{ document.id }}</td>
            <td>{{ document.type }}</td>
            <td>{{ formatDate(document.date) }}</td>
            <td>{{ document.firstName }}</td>
            <td>{{ document.lastName }}</td>
            <td>{{ document.city }}</td>
            <td>
              <button @click="showDetails(document.id)">Details</button>
              <button @click="openUpdateDocumentModal(document)">Update</button>
              <button @click="deleteDocument(document.id)">Delete</button>
            </td>
          </tr>
        </tbody>
      </table>
      <pagination
          v-if="totalPages > 0"
          :total-pages="totalPages"
          :current-page="pageIndex"
          @change-page="handleChangePage"
      />
      </div>
      <div v-else class="documents-not-found">
        ERR 404: DOCUMENTS NOT FOUND.
      </div>
      <details-modal
        :visible="detailsModalVisible"
        :document-items="documentItems"
        :document-id="currentDocumentId"
        @close="() => detailsModalVisible = false"
      />
      <add-document-modal
        v-if="state.isDataLoaded"
        :visible="addDocumentModalVisible"
        :user-has-data="state.userHasData"
        :user-data="userData"
        @close="() => addDocumentModalVisible = false"
      />
      <update-document-modal
        :modelValue="selectedDocument"
        :visible="updateDocumentModalVisible"
        @update:modelValue="handleUpdate"
        @close="updateDocumentModalVisible = false"
      />
    </div>
</template>

<style scoped>

@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

.documents {
  font-size: 0.8rem;
  margin: 1rem;
  animation: fadeIn 2s ease;
}

.search {
  display: flex;
  gap: 0.5rem;
  margin-bottom: 1rem;
}

.search input {
  flex-grow: 1;
  font-size: 1rem;
  padding: 0.5rem;
}

button {
  font-size: 1rem;
  padding: 0.5rem 1rem;
  background-color: #151515;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

button:hover {
  background-color: #0056b3;
}

table {
  width: 100%;
  border-collapse: collapse;
}

th, td {
  padding: 8px;
  text-align: left;
  border-bottom: 1px solid #ddd;
}

th {
  background-color: #36013F;
  color: white;
}

td button {
  margin-left: 0.5rem;
}

.documents-not-found {
  text-align: center;
  font-size: 2rem;
  margin-top: 2rem;
  color: #ccc;
  animation: fadeIn 2s ease;
}

@media (max-width: 768px) {
  .documents {
    font-size: 1rem;
  }

  th, td {
    padding: 12px;
  }
}
</style>
