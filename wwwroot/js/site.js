const themeToggle = document.querySelector('[data-theme-toggle]');
const themeLabel = document.querySelector('[data-theme-label]');
const searchForm = document.querySelector('.search-card');
const searchInput = document.querySelector('#driver-search');
const searchStatus = document.querySelector('[data-search-status]');
const filterChips = document.querySelectorAll('[data-filter-chip]');
const storedTheme = localStorage.getItem('verify-driver-theme');
const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;

function setTheme(theme) {
	document.documentElement.dataset.theme = theme;
	localStorage.setItem('verify-driver-theme', theme);

	if (themeLabel) {
		themeLabel.textContent = theme === 'dark' ? 'Light' : 'Dark';
	}
}

setTheme(storedTheme || (prefersDark ? 'dark' : 'light'));

themeToggle?.addEventListener('click', () => {
	const nextTheme = document.documentElement.dataset.theme === 'dark' ? 'light' : 'dark';
	setTheme(nextTheme);
});

searchForm?.addEventListener('submit', (event) => {
	event.preventDefault();

	const query = searchInput?.value.trim();

	if (!query) {
		searchStatus.textContent = 'Enter a driver name, registration, platform, or city to start verification.';
		return;
	}

	searchStatus.textContent = `Showing trust signals related to "${query}". Live API search can replace this preview once the driver endpoint supports query filters.`;
});

filterChips.forEach((chip) => {
	chip.addEventListener('click', () => {
		filterChips.forEach((item) => item.classList.remove('is-active'));
		chip.classList.add('is-active');
	});
});
