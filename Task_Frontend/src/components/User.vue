<script setup>
import { ref, onMounted } from 'vue';
import userDataService from '@/services/UserDataService.js';

const userData = ref({
  firstName: '',
  lastName: '',
  city: ''
});
let hasData = ref(false);
const loading = ref(true);
const editing = ref(false);

const checkUserData = async () => {
  try {
    const response = await userDataService.hasUserData();
    hasData = response.data;
    if (hasData) {
      loading.value = false;
      await loadUserData();
    } else {
      loading.value = false;
    }
  } catch (error) {
    console.error('Error checking user data:', error);
    loading.value = false;
  }
};

const loadUserData = async () => {
  try {
    const response = await userDataService.getActualUserData();
    for (const key in response.data) {
      userData.value[key] = response.data[key];
    }
    loading.value = false;
  } catch (error) {
    console.error('Error loading user data:', error);
    loading.value = false;
  }
};

const submitData = async () => {
  try {
    const response = await userDataService.addUserData(userData.value);
    hasData = true;
    window.location.reload();
    window.alert("Data submitted");
  } catch (error) {
    console.error('Error submitting data:', error);
  }
};

const startEditing = () => {
  editing.value = true;
};

const saveData = async () => {
  try {
    await userDataService.updateUserData(userData.value);
    editing.value = false;
    window.location.reload();
    window.alert("Data changed");
  } catch (error) {
    console.error('Error updating data:', error);
  }
};

onMounted(checkUserData);
</script>


<template>
  <div class="page-container">
    <div v-if="loading">
      Loading...
    </div>
    <div v-else>
      <div v-if="hasData && !editing">
        <h3>Your Data:</h3>
        <p>First Name: {{ userData.firstName }}</p>
        <p>Last Name: {{ userData.lastName }}</p>
        <p>City: {{ userData.city }}</p>
        <button @click="startEditing">Update Data</button>
      </div>
      <div v-else class="user-form">
        <h3 v-if="editing">Edit your data:</h3>
        <h3 v-else>Please enter your data:</h3>
        <form @submit.prevent="editing ? saveData() : submitData()">
          <label>First Name:</label>
          <input type="text" v-model="userData.firstName" required />

          <label>Last Name:</label>
          <input type="text" v-model="userData.lastName" required />

          <label>City:</label>
          <input type="text" v-model="userData.city" required />

          <button type="submit">{{ editing ? 'Save Changes' : 'Submit' }}</button>
        </form>
      </div>
    </div>
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

.page-container {
  display: flex;
  justify-content: center;
  align-items: center;
  font-family: 'Arial', sans-serif;
  margin-top: 20px;
  color: #FFF;
  animation: fadeIn 2s ease;
}

.user-form {
  width: 350px; /* Smaller width for the form */
  padding: 2rem;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.5);
  border-radius: 5px;
  background: linear-gradient(145deg, #36013F, #3F3F3F);
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  animation: fadeIn 2s ease;
}

.user-form h3 {
  margin-bottom: 1rem;
  color: #FFF;
}

input[type="text"] {
  width: 90%;
  padding: 12px;
  margin: 10px 0;
  border: 1px solid #555;
  border-radius: 4px;
  background-color: #1A1A1A;
  color: #FFF;
  font-size: 16px;
}

input::placeholder {
  color: #AAA;
}

button {
  padding: 12px 20px;
  margin-top: 20px;
  border: none;
  border-radius: 4px;
  background-color: #5C6BC0;
  color: white;
  cursor: pointer;
  transition: background-color 0.3s;
  align-self: center;
  animation: fadeIn 2s ease;
}

button:hover {
  background-color: #3F51B5;
}

@media (max-width: 768px) {
  .user-form {
    width: 90%;
    margin: 20px;
  }

  input[type="text"],
  button {
    width: 100%;
  }
}
</style>