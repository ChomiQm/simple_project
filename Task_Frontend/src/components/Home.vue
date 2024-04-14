<script>
import { ref, onMounted, onUnmounted } from 'vue';

export default {
  name: "Home",
  setup() {
    const activeSection = ref(0);
    const sectionCount = 2;
    let intervalId;

    onMounted(() => {
      intervalId = setInterval(() => {
        activeSection.value = (activeSection.value + 1) % sectionCount;
      }, 15000);
    });

    onUnmounted(() => {
      clearInterval(intervalId);
    });

    return { activeSection };
  }
};
</script>

<template>
  <div class="home">
    <transition name="fade" mode="out-in">
      <section v-if="activeSection === 0" key="1" class="section-content">
        <!-- Zawartość Sekcji 1 -->
        <section class="section">
          <img src="@/assets/Vuejs-logo.svg" alt="Vue.js Logo" class="logo" />
          <div class="content">
            <h2>Vue.js</h2>
            <p>Vue.js is a progressive framework for building user interfaces. In this project, we utilize Vue.js version 3, which ensures reactivity and composition API syntax.</p>
          </div>
        </section>
        <section class="section reverse">
          <div class="content">
            <h2>Vite</h2>
            <p>Vite is a modern build tool that provides a fast development server with Hot Module Replacement. We use Vite for quick project start-up and production optimization.</p>
          </div>
          <img src="@/assets/Vitejs-logo.svg" alt="Vite Logo" class="logo" />
        </section>
        <section class="section">
          <img src="@/assets/Csharp.svg" alt="C# Logo" class="logo" />
          <div class="content">
            <h2>C# and .NET 8.0</h2>
            <p>C# is a programming language created by Microsoft. We use it in conjunction with the latest .NET 8.0 framework, which allows us to build an efficient API and manage authorization through Identity.</p>
          </div>
        </section>
        <section class="section reverse">
          <div class="content">
            <h2>SQL Server</h2>
            <p>Microsoft SQL Server is an advanced database management system. We utilize it for data storage and management, ensuring security and scalability of the application.</p>
          </div>
          <img src="@/assets/Mssql.svg" alt="MSSQL Logo" class="logo" />
        </section>
      </section>
      <section v-else key="2" class="json-container">
        <!-- Zawartość Sekcji 2 -->
        <pre class="json-code">
          <span class="json-key">"ConnectionStrings":</span> {
            <span class="json-key">"DefaultConnection":</span> "base_conn_string"
          },
          <span class="json-key">"Logging":</span> {
            <span class="json-key">"LogLevel":</span> {
              <span class="json-key">"Default":</span> "Information",
              <span class="json-key">"Microsoft.AspNetCore":</span> "Warning"
            }
          },
          <span class="json-key">"AllowedHosts":</span> "*"
        </pre>
        <p class="json-description">Sample <code>appsettings.json</code> used for backend build.</p>
      </section>
    </transition>
  </div>
</template>

<style scoped>
.home {
  background: rgba(45, 45, 45, 0.1);
  text-align: center;
  max-width: 700px;
  margin: auto;
  padding: 2rem;
}

.section {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 4rem;
}

.section h2 {
  position: relative;
  padding: 0 1rem;
}

.section h2::before {
  content: '';
  position: absolute;
  bottom: -10px;
  left: 0;
  right: 0;
  height: 2px;
  background: #6435c9;
  transform: scaleX(0);
  transition: transform 0.3s ease-in-out;
}

.section:hover h2::before {
  transform: scaleX(1);
}

.logo {
  width: 80px;
  transition: transform 0.3s ease-in-out;
}

.logo:hover {
  transform: scale(1.1);
}

.content {
  flex-grow: 1;
  padding: 0 1rem;
}

.content h2 {
  font-size: 1.15rem;
}

.content p {
  font-size: 1.05rem;
}

.json-key {
  color: #FCAE1E;
}

.json-container {
  display: flex;
  justify-content: center;
  flex-direction: column;
  align-items: center;
}

.json-description {
    color: #FFF;
    font-size: 0.9rem;
    text-align: center;
    max-width: 600px;
  }

.json-code {
  width: 83%;
  max-width: 600px;
  margin: 30px;
  padding: 1.2rem;
  background: rgba(255, 255, 255, 0.3);
  border-radius: 10px;
  border: 1px solid rgba(255, 255, 255, 0.05);
  box-shadow: 0 8px 32px 0 rgba(31, 38, 135, 0.9);
  backdrop-filter: blur(7px);
  -webkit-backdrop-filter: blur(4px);
  color: #FA8128;
  font-size: 14px;
  line-height: 1.6;
  overflow:auto;
  text-align: left;
  white-space: pre;
  font-family: 'Courier New', Courier, monospace;
  transform: translateY(-5%);
}

.fade-enter-active, .fade-leave-active {
  transition: opacity 1s;
}
.fade-enter-from, .fade-leave-to {
  opacity: 0;
}

@media (max-width: 768px) {
  .section {
    flex-direction: column;
  }

   .json-code {
    width: 70%;
  }

  .section h2::before {
    display: none;
  }

  .content {
    padding: 0;
    text-align: center;
  }
}
</style>


