/* ===== Products Listing JavaScript ===== */

(function () {
  "use strict";

  // DOM Elements
  const filterForm = document.querySelector(".product-filter-form");
  const searchInput = document.querySelector(".search-input");
  const categoryFilter = document.querySelector(".category-filter");
  const supplierFilter = document.querySelector(".supplier-filter");
  const sortFilter = document.querySelector(".sort-filter");
  const filterSubmitBtn = document.querySelector(".filter-submit-btn");
  const productCards = document.querySelectorAll(".product-card");
  const addToCartBtns = document.querySelectorAll(".add-to-cart-btn");
  const productsGrid = document.querySelector(".products-grid");

  /**
   * Initialize Product Listing
   */
  function init() {
    setupEventListeners();
    attachProductCardAnimations();
    setupFormValidation();
    debounceFilterChanges();
  }

  /**
   * Setup Event Listeners
   */
  function setupEventListeners() {
    // Form submission
    if (filterForm) {
      filterForm.addEventListener("submit", handleFormSubmit);
    }

    // Real-time search
    if (searchInput) {
      searchInput.addEventListener("input", handleSearchInput);
    }

    // Filter changes
    [categoryFilter, supplierFilter, sortFilter].forEach((el) => {
      if (el) {
        el.addEventListener("change", handleFilterChange);
      }
    });

    // Add to cart buttons
    addToCartBtns.forEach((btn) => {
      btn.addEventListener("click", handleAddToCart);
    });

    // Quick actions hover effect
    productCards.forEach((card) => {
      card.addEventListener("mouseenter", function () {
        this.classList.add("active");
      });
      card.addEventListener("mouseleave", function () {
        this.classList.remove("active");
      });
    });
  }

  /**
   * Handle form submission
   */
  function handleFormSubmit(e) {
    const hasFilters = isAnyFilterActive();

    if (!hasFilters) {
      // Show warning if no filters selected
      showNotification("Vui lòng chọn ít nhất một bộ lọc", "warning");
      e.preventDefault();
    }

    animateButtonSubmit(filterSubmitBtn);
  }

  /**
   * Handle search input
   */
  function handleSearchInput(e) {
    const searchValue = e.target.value.trim();

    // Clear previous timeouts
    if (window.searchTimeout) {
      clearTimeout(window.searchTimeout);
    }

    // Auto-submit after 500ms of inactivity
    window.searchTimeout = setTimeout(() => {
      if (searchValue.length >= 2 || searchValue.length === 0) {
        filterForm.submit();
      }
    }, 500);
  }

  /**
   * Handle filter changes
   */
  function handleFilterChange(e) {
    const el = e.target;

    // Add visual feedback
    el.classList.add("changed");

    // Reset timeout if exists
    if (window.filterTimeout) {
      clearTimeout(window.filterTimeout);
    }

    // Auto-submit after 300ms
    window.filterTimeout = setTimeout(() => {
      filterForm.submit();
    }, 300);
  }

  /**
   * Handle add to cart button clicks
   */
  function handleAddToCart(e) {
    e.preventDefault();

    const btn = e.currentTarget;
    const productId = btn.closest(".product-card").dataset.productId;
    const productName = btn
      .closest(".product-card")
      .querySelector(".product-title").textContent;

    // Button feedback
    const originalContent = btn.innerHTML;
    btn.innerHTML = '<i class="fas fa-check"></i>';
    btn.classList.add("disabled");

    // Show notification
    showNotification(
      `Chuyển hướng đến chi tiết sản phẩm: ${productName}`,
      "success",
    );

    // Reset button after animation
    setTimeout(() => {
      btn.innerHTML = originalContent;
      btn.classList.remove("disabled");
    }, 1500);
  }

  /**
   * Attach animations to product cards
   */
  function attachProductCardAnimations() {
    productCards.forEach((card, index) => {
      card.style.animationDelay = `${index * 0.1}s`;

      // Intersection Observer for lazy animation
      observeCardInView(card);
    });
  }

  /**
   * Observe card in viewport
   */
  function observeCardInView(card) {
    if ("IntersectionObserver" in window) {
      const observer = new IntersectionObserver(
        (entries) => {
          entries.forEach((entry) => {
            if (entry.isIntersecting) {
              entry.target.classList.add("in-view");
              observer.unobserve(entry.target);
            }
          });
        },
        { threshold: 0.1 },
      );

      observer.observe(card);
    }
  }

  /**
   * Setup form validation
   */
  function setupFormValidation() {
    if (filterForm) {
      filterForm.addEventListener(
        "invalid",
        (e) => {
          e.preventDefault();
          showNotification(
            "Vui lòng kiểm tra lại các trường nhập liệu",
            "danger",
          );
        },
        true,
      );
    }
  }

  /**
   * Debounce filter changes
   */
  function debounceFilterChanges() {
    let isFiltering = false;

    if (filterSubmitBtn) {
      filterSubmitBtn.addEventListener("click", () => {
        if (isFiltering) return;

        isFiltering = true;
        showLoadingState(true);

        setTimeout(() => {
          isFiltering = false;
        }, 1000);
      });
    }
  }

  /**
   * Check if any filter is active
   */
  function isAnyFilterActive() {
    const search = searchInput ? searchInput.value.trim() : "";
    const category = categoryFilter ? categoryFilter.value : "0";
    const supplier = supplierFilter ? supplierFilter.value : "0";
    const sort = sortFilter ? sortFilter.value : "";

    return search || category !== "0" || supplier !== "0" || sort;
  }

  /**
   * Animate button submit
   */
  function animateButtonSubmit(btn) {
    if (!btn) return;

    btn.classList.add("submitted");
    const originalContent = btn.innerHTML;

    btn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Đang tìm...';
    btn.disabled = true;

    setTimeout(() => {
      btn.innerHTML = originalContent;
      btn.disabled = false;
      btn.classList.remove("submitted");
    }, 1500);
  }

  /**
   * Show loading state
   */
  function showLoadingState(show) {
    if (productsGrid) {
      if (show) {
        productsGrid.style.opacity = "0.5";
        productsGrid.style.pointerEvents = "none";
      } else {
        productsGrid.style.opacity = "1";
        productsGrid.style.pointerEvents = "auto";
      }
    }
  }

  /**
   * Show notification
   */
  function showNotification(message, type = "info") {
    // Create notification element
    const notification = document.createElement("div");
    notification.className = `alert alert-${type} alert-dismissible fade show`;
    notification.role = "alert";
    notification.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            z-index: 9999;
            max-width: 400px;
            animation: slideInRight 0.3s ease-out;
        `;

    const iconMap = {
      success: "fa-check-circle",
      danger: "fa-exclamation-circle",
      warning: "fa-exclamation-triangle",
      info: "fa-info-circle",
    };

    const icon = iconMap[type] || "fa-info-circle";

    notification.innerHTML = `
            <i class="fas ${icon} me-2"></i>
            <span>${message}</span>
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        `;

    document.body.appendChild(notification);

    // Auto dismiss after 4 seconds
    setTimeout(() => {
      notification.classList.remove("show");
      setTimeout(() => {
        notification.remove();
      }, 300);
    }, 4000);
  }

  /**
   * Format price
   */
  function formatPrice(price) {
    return new Intl.NumberFormat("vi-VN", {
      style: "currency",
      currency: "VND",
    }).format(price);
  }

  /**
   * Setup product hover effects
   */
  function setupProductHoverEffects() {
    productCards.forEach((card) => {
      const image = card.querySelector(".product-image");
      const overlay = card.querySelector(".product-overlay");

      if (image && overlay) {
        card.addEventListener("mouseenter", () => {
          overlay.style.opacity = "1";
        });

        card.addEventListener("mouseleave", () => {
          overlay.style.opacity = "0";
        });
      }
    });
  }

  /**
   * Setup keyboard navigation
   */
  function setupKeyboardNavigation() {
    document.addEventListener("keydown", (e) => {
      // Alt + N = Focus on search
      if (e.altKey && e.code === "KeyN") {
        e.preventDefault();
        if (searchInput) {
          searchInput.focus();
        }
      }

      // Alt + F = Focus on first filter
      if (e.altKey && e.code === "KeyF") {
        e.preventDefault();
        if (categoryFilter) {
          categoryFilter.focus();
        }
      }
    });
  }

  /**
   * Smooth scrolling
   */
  function setupSmoothScroll() {
    document.querySelectorAll('a[href^="#"]').forEach((anchor) => {
      anchor.addEventListener("click", function (e) {
        e.preventDefault();
        const target = document.querySelector(this.getAttribute("href"));
        if (target) {
          target.scrollIntoView({
            behavior: "smooth",
            block: "start",
          });
        }
      });
    });
  }

  /**
   * Setup localStorage for filter preferences
   */
  function setupFilterPreferences() {
    const savePreferences = () => {
      const prefs = {
        search: searchInput ? searchInput.value : "",
        category: categoryFilter ? categoryFilter.value : "0",
        supplier: supplierFilter ? supplierFilter.value : "0",
        sort: sortFilter ? sortFilter.value : "",
      };
      localStorage.setItem("productFilterPrefs", JSON.stringify(prefs));
    };

    const loadPreferences = () => {
      const prefs = JSON.parse(
        localStorage.getItem("productFilterPrefs") || "{}",
      );
      if (prefs.search && searchInput) searchInput.value = prefs.search;
      if (prefs.category && categoryFilter)
        categoryFilter.value = prefs.category;
      if (prefs.supplier && supplierFilter)
        supplierFilter.value = prefs.supplier;
      if (prefs.sort && sortFilter) sortFilter.value = prefs.sort;
    };

    // Save preferences on change
    [searchInput, categoryFilter, supplierFilter, sortFilter].forEach((el) => {
      if (el) {
        el.addEventListener("change", savePreferences);
      }
    });

    // Load preferences on page load
    loadPreferences();
  }

  /**
   * Initialize on DOM ready
   */
  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", init);
  } else {
    init();
  }

  // Export for external use
  window.ProductListing = {
    formatPrice,
    showNotification,
    isAnyFilterActive,
  };
})();

/* ===== Product Form & Delete JavaScript ===== */

(function () {
  "use strict";

  /**
   * Initialize Form Handlers
   */
  function initFormHandlers() {
    setupProductFormValidation();
    setupCategorySelectorAutoSubmit();
    setupDeleteConfirmation();
    setupFormAnimations();
  }

  /**
   * Setup Product Form Validation
   */
  function setupProductFormValidation() {
    const productForms = document.querySelectorAll(".product-form");

    productForms.forEach((form) => {
      // Add validation feedback
      form.addEventListener("submit", function (e) {
        if (!form.checkValidity()) {
          e.preventDefault();
          e.stopPropagation();
          showFormNotification(
            "Vui lòng điền đầy đủ các trường bắt buộc",
            "danger",
          );
        }
        form.classList.add("was-validated");
      });

      // Real-time validation feedback
      const inputs = form.querySelectorAll(
        "input[required], select[required], textarea[required]",
      );
      inputs.forEach((input) => {
        input.addEventListener("blur", function () {
          validateField(this);
        });

        input.addEventListener("change", function () {
          if (form.classList.contains("was-validated")) {
            validateField(this);
          }
        });
      });

      // Add field animation
      const formGroups = form.querySelectorAll(".form-group");
      formGroups.forEach((group, index) => {
        group.style.animationDelay = `${index * 0.05}s`;
      });
    });
  }

  /**
   * Validate individual field
   */
  function validateField(field) {
    const isValid = field.checkValidity();
    const feedback = field.closest(".form-group").querySelector(".text-danger");

    if (isValid) {
      field.classList.remove("is-invalid");
      field.classList.add("is-valid");
      if (feedback) {
        feedback.style.display = "none";
      }
    } else {
      field.classList.remove("is-valid");
      field.classList.add("is-invalid");
      if (feedback) {
        feedback.style.display = "block";
      }
    }
  }

  /**
   * Setup Category Selector Auto Submit
   */
  function setupCategorySelectorAutoSubmit() {
    const categorySelector = document.querySelector(".category-selector");

    if (categorySelector) {
      categorySelector.addEventListener("change", function () {
        const form = this.closest(".category-selector-form");
        if (form) {
          form.submit();
        }
      });
    }
  }

  /**
   * Setup Delete Confirmation
   */
  function setupDeleteConfirmation() {
    const confirmCheckbox = document.getElementById("confirmDelete");
    const submitBtn = document.querySelector(".delete-submit-btn");

    if (confirmCheckbox && submitBtn) {
      // Disable button initially
      submitBtn.disabled = true;

      // Toggle button on checkbox
      confirmCheckbox.addEventListener("change", function () {
        submitBtn.disabled = !this.checked;

        if (this.checked) {
          submitBtn.classList.add("pulse-enable");
          setTimeout(() => {
            submitBtn.classList.remove("pulse-enable");
          }, 600);
        }
      });

      // Add form submit handler
      const deleteForm = document.querySelector(".delete-confirmation-form");
      if (deleteForm) {
        deleteForm.addEventListener("submit", function (e) {
          if (!confirmCheckbox.checked) {
            e.preventDefault();
            showFormNotification("Vui lòng xác nhận trước khi xóa", "danger");
          }
        });
      }
    }
  }

  /**
   * Setup Form Animations
   */
  function setupFormAnimations() {
    const detailRows = document.querySelectorAll(".detail-row");

    detailRows.forEach((row, index) => {
      row.style.animation = `fadeInUp 0.5s ease-out ${index * 0.05}s backwards`;
    });

    // Intersection Observer for details cards
    const detailsCards = document.querySelectorAll(".details-card");
    if ("IntersectionObserver" in window) {
      const observer = new IntersectionObserver(
        (entries) => {
          entries.forEach((entry) => {
            if (entry.isIntersecting) {
              entry.target.classList.add("in-view");
              observer.unobserve(entry.target);
            }
          });
        },
        { threshold: 0.1 },
      );

      detailsCards.forEach((card) => {
        observer.observe(card);
      });
    }
  }

  /**
   * Show form notification
   */
  function showFormNotification(message, type = "info") {
    const notification = document.createElement("div");
    notification.className = `alert alert-${type} alert-dismissible fade show`;
    notification.role = "alert";
    notification.style.cssText = `
      position: fixed;
      top: 20px;
      right: 20px;
      z-index: 9999;
      max-width: 450px;
      animation: slideInRight 0.3s ease-out;
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
    `;

    const iconMap = {
      success: "fa-check-circle",
      danger: "fa-exclamation-circle",
      warning: "fa-exclamation-triangle",
      info: "fa-info-circle",
    };

    const icon = iconMap[type] || "fa-info-circle";

    notification.innerHTML = `
      <i class="fas ${icon} me-2"></i>
      <span>${message}</span>
      <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    `;

    document.body.appendChild(notification);

    // Auto dismiss after 5 seconds
    setTimeout(() => {
      notification.classList.remove("show");
      setTimeout(() => {
        notification.remove();
      }, 300);
    }, 5000);
  }

  /**
   * Setup image preview for URL input
   */
  function setupImagePreview() {
    const imageInputs = document.querySelectorAll('input[type="url"]');

    imageInputs.forEach((input) => {
      input.addEventListener("blur", function () {
        if (this.value && this.value.startsWith("http")) {
          const previewContainer = this.closest(".form-group");
          let preview = previewContainer.querySelector(".image-preview");

          if (!preview) {
            preview = document.createElement("img");
            preview.className = "image-preview";
            preview.style.cssText = `
              margin-top: 1rem;
              max-width: 200px;
              max-height: 200px;
              border-radius: 10px;
              box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
            `;
            previewContainer.appendChild(preview);
          }

          preview.src = this.value;
          preview.onerror = function () {
            preview.style.display = "none";
          };
        }
      });
    });
  }

  /**
   * Setup Number Input Formatting
   */
  function setupNumberFormatting() {
    const numberInputs = document.querySelectorAll(
      'input[type="number"][asp-for*="Price"], input[type="number"][asp-for*="Stock"]',
    );

    numberInputs.forEach((input) => {
      input.addEventListener("blur", function () {
        if (this.value) {
          const num = parseInt(this.value);
          if (!isNaN(num)) {
            this.value = num;
          }
        }
      });
    });
  }

  /**
   * Setup form field focus effects
   */
  function setupFieldFocusEffects() {
    const formControls = document.querySelectorAll(
      ".form-control, .form-select",
    );

    formControls.forEach((control) => {
      control.addEventListener("focus", function () {
        this.parentElement.classList.add("focused");
      });

      control.addEventListener("blur", function () {
        this.parentElement.classList.remove("focused");
      });
    });
  }

  /**
   * Initialize on DOM ready
   */
  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", function () {
      initFormHandlers();
      setupImagePreview();
      setupNumberFormatting();
      setupFieldFocusEffects();
    });
  } else {
    initFormHandlers();
    setupImagePreview();
    setupNumberFormatting();
    setupFieldFocusEffects();
  }

  // Add CSS animations if not already defined
  if (!document.querySelector("style[data-form-animations]")) {
    const style = document.createElement("style");
    style.setAttribute("data-form-animations", "true");
    style.innerHTML = `
      @keyframes slideInRight {
        from {
          opacity: 0;
          transform: translateX(20px);
        }
        to {
          opacity: 1;
          transform: translateX(0);
        }
      }

      @keyframes fadeInUp {
        from {
          opacity: 0;
          transform: translateY(10px);
        }
        to {
          opacity: 1;
          transform: translateY(0);
        }
      }

      .form-control.is-valid,
      .form-select.is-valid {
        border-color: #28a745;
        background-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 8 8'%3e%3cpath fill='%2328a745' d='M2.3 6.73L.6 4.53c-.4-1.04.46-1.4 1.1-.8l1.1 1.4 3.4-3.8c.6-.63 1.6-.27 1.2.7l-4 4.6c-.43.5-.8.4-1.1.1z'/%3e%3c/svg%3e");
        background-position: right calc(0.375em + 0.1875rem) center;
        background-repeat: no-repeat;
        background-size: calc(0.75em + 0.375rem) calc(0.75em + 0.375rem);
        padding-right: calc(1.5em + 0.75rem);
      }

      .form-control.is-invalid,
      .form-select.is-invalid {
        border-color: #dc3545;
        background-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' width='12' height='12' fill='none' stroke='%23dc3545' viewBox='0 0 12 12'%3e%3ccircle cx='6' cy='6' r='4.5'/%3e%3cpath stroke-linejoin='round' d='M5.8 3.6h.4L6 6.5z'/%3e%3ccircle cx='6' cy='8.2' r='.6' fill='%23dc3545' stroke='none'/%3e%3c/svg%3e");
        background-position: right calc(0.375em + 0.1875rem) center;
        background-repeat: no-repeat;
        background-size: calc(0.75em + 0.375rem) calc(0.75em + 0.375rem);
        padding-right: calc(1.5em + 0.75rem);
      }

      .delete-submit-btn:disabled {
        opacity: 0.6;
        cursor: not-allowed;
      }

      .delete-submit-btn.pulse-enable {
        animation: pulse 0.6s ease-out;
      }

      @keyframes pulse {
        0% {
          transform: scale(1);
        }
        50% {
          transform: scale(1.05);
        }
        100% {
          transform: scale(1);
        }
      }
    `;
    document.head.appendChild(style);
  }
})();
