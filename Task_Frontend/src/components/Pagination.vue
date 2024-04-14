<script>
export default {
  props: {
    totalPages: {
      type: Number,
      required: true
    },
    currentPage: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      windowSize: 5 // Ile stron wyświetlać na raz
    }
  },
  computed: {
    // Obliczane strony do wyświetlenia
    pagesToShow() {
      const halfWindow = Math.floor(this.windowSize / 2);
      let start = Math.max(this.currentPage - halfWindow, 0);
      let end = Math.min(start + this.windowSize, this.totalPages);

      // Dostosuj okno, jeśli jest na końcu listy
      if (end >= this.totalPages) {
        start = Math.max(this.totalPages - this.windowSize, 0);
        end = this.totalPages;
      }

      return Array.from({ length: end - start }, (_, i) => i + start);
    }
  },
  methods: {
    changePage(page) {
      if (page < 0 || page >= this.totalPages) return;
      this.$emit('change-page', page);
    }
  }
};
</script>

<template>
  <nav aria-label="Page navigation">
    <ul class="pagination">
      <!-- Przycisk poprzedni -->
      <li class="page-item" :class="{ disabled: currentPage === 0 }">
        <a class="page-link" href="#" aria-label="Previous" @click="changePage(currentPage - 1)">
          <span aria-hidden="true">&laquo;</span>
        </a>
      </li>
      <!-- Strony -->
      <li class="page-item" v-for="page in pagesToShow" :class="{ active: page === currentPage }" :key="page">
        <a class="page-link" href="#" @click="changePage(page)">
          {{ page + 1 }}
        </a>
      </li>
      <!-- Przycisk następny -->
      <li class="page-item" :class="{ disabled: currentPage >= totalPages - 1 }">
        <a class="page-link" href="#" aria-label="Next" @click="changePage(currentPage + 1)">
          <span aria-hidden="true">&raquo;</span>
        </a>
      </li>
    </ul>
  </nav>
</template>


<style>
.pagination {
  display: flex;
  list-style: none;
  padding-left: 0;
}

.page-item {
  margin: 0 2px;
}

.page-item.disabled .page-link {
  color: grey;
}

.page-item.active .page-link {
  background-color: blue;
  color: white;
}

.page-link {
  display: block;
  padding: 5px 10px;
  text-decoration: none;
  cursor: pointer;
}
</style>