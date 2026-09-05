const themeToggle = document.querySelector('[data-theme-toggle]');
const themeLabel = document.querySelector('[data-theme-label]');
const searchForm = document.querySelector('.search-card');
const searchInput = searchForm?.querySelector('input[type="search"]');
const searchStatus = document.querySelector('[data-search-status]');
const searchModeTabs = document.querySelectorAll('[data-search-mode-tab]');
const profileSearchLabel = document.querySelector('[data-profile-search-label]');
const opportunityControls = document.querySelector('[data-opportunity-controls]');
const intentSelect = document.querySelector('[data-intent-select]');
const relationshipSelect = document.querySelector('[data-relationship-select]');
const verificationForm = document.querySelector('[data-verification-form]');
const verificationStatus = document.querySelector('[data-verification-status]');
const evidenceStatus = document.querySelector('[data-evidence-status]');
const evidenceInputs = document.querySelectorAll('[data-evidence-input]');
const verificationChecks = document.querySelectorAll('[data-verify-check]');
const verifyProfile = document.querySelector('[data-verify-profile]');
const verifyContext = document.querySelector('[data-verify-context]');
const verifyCounterparty = document.querySelector('[data-verify-counterparty]');
const caseType = document.querySelector('[data-case-type]');
const trustCueScore = document.querySelector('[data-trust-cue-score]');
const trustCueName = document.querySelector('[data-trust-cue-name]');
const trustCueSummary = document.querySelector('[data-trust-cue-summary]');
const trustCueRisk = document.querySelector('[data-trust-cue-risk]');
const trustCueLinks = document.querySelector('[data-trust-cue-links]');
const profileComparison = document.querySelector('[data-profile-comparison]');
const comparisonResults = document.querySelector('[data-comparison-results]');
const filterChips = document.querySelectorAll('[data-filter-chip]');
const profilePanel = document.querySelector('[data-profile-panel]');
const profileStatus = document.querySelector('[data-profile-status]');
const profileName = document.querySelector('[data-profile-name]');
const profileMeta = document.querySelector('[data-profile-meta]');
const profileTrust = document.querySelector('[data-profile-trust]');
const profileReports = document.querySelector('[data-profile-reports]');
const profileRisk = document.querySelector('[data-profile-risk]');
const profileLinks = document.querySelector('[data-profile-links]');
const feedEmpty = document.querySelector('[data-feed-empty]');
const feedResults = document.querySelector('[data-feed-results]');
const signalForm = document.querySelector('[data-signal-form]');
const signalCategory = document.querySelector('[data-signal-category]');
const signalSeverity = document.querySelector('[data-signal-severity]');
const signalContext = document.querySelector('[data-signal-context]');
const signalStatus = document.querySelector('[data-signal-status]');
const signalDraft = document.querySelector('[data-signal-draft]');
const storedTheme = localStorage.getItem('verify-driver-theme');
const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;

function setTheme(theme) {
	document.documentElement.dataset.theme = theme;
	localStorage.setItem('verify-driver-theme', theme);

	if (themeLabel) {
		themeLabel.textContent = theme === 'dark' ? 'Light' : 'Dark';
	}
}

async function submitFeedbackSignal(context) {
	const feedbackUrl = signalForm?.dataset.feedbackUrl;

	if (!feedbackUrl) {
		updateSignalStatus('Feedback submission is not configured yet.', 'warning');
		return;
	}

	try {
		const response = await fetch(feedbackUrl, {
			method: 'POST',
			headers: { 'Content-Type': 'application/json', Accept: 'application/json' },
			body: JSON.stringify({
				category: signalCategory?.value || 'Public feedback',
				severity: Number.parseInt(signalSeverity?.value || '1', 10),
				context,
				relatedProfileId: Number.parseInt(profilePanel?.dataset.userId || '0', 10) || null,
				relatedEntity: profileName?.textContent || null,
				submitterType: 'Public'
			})
		});
		const result = await response.json();
		updateSignalStatus(result.message || 'Signal submitted to moderation.', response.ok ? 'success' : 'warning');
	} catch (error) {
		updateSignalStatus('Feedback API is unavailable. Save the details and retry when VerifyDriverAPI is running.', 'warning');
		console.warn(error);
	}
}

async function submitVerificationCase(profileId, selectedEvidenceCount, checkedCount) {
	const verificationUrl = verificationForm?.dataset.verificationUrl;

	if (!verificationUrl || !verificationStatus) {
		return;
	}

	verificationStatus.textContent = 'Submitting verification case to VerifyDriverAPI...';

	try {
		const response = await fetch(verificationUrl, {
			method: 'POST',
			headers: { 'Content-Type': 'application/json', Accept: 'application/json' },
			body: JSON.stringify({
				caseType: caseType?.value || 'Relationship verification',
				relationshipContext: verifyContext?.value || 'Relationship verification',
				primaryProfileId: profileId,
				counterparty: verifyCounterparty?.value || null,
				evidence: Array.from(evidenceInputs).flatMap((input) => Array.from(input.files || []).map((file) => ({
					documentType: input.closest('.evidence-card')?.querySelector('span')?.textContent || 'Evidence',
					fileName: file.name,
					contentType: file.type || null,
					sizeBytes: file.size
				}))),
				confirmations: Array.from(verificationChecks).map((item) => ({
					counterparty: verifyCounterparty?.value || 'Counterparty',
					claim: item.parentElement?.textContent?.trim() || 'Relationship confirmation',
					state: item.checked ? 'Confirmed' : 'Requested'
				}))
			})
		});
		const result = await response.json();
		verificationStatus.textContent = result.message || (response.ok
			? `Verification case created with ${selectedEvidenceCount} document${selectedEvidenceCount === 1 ? '' : 's'} and ${checkedCount} confirmation check${checkedCount === 1 ? '' : 's'}.`
			: 'Verification case could not be created.');
	} catch (error) {
		verificationStatus.textContent = 'Verification case API is unavailable. Keep the evidence packet and retry when VerifyDriverAPI is running.';
		console.warn(error);
	}
}

setTheme(storedTheme || (prefersDark ? 'dark' : 'light'));
configureRelationshipOptions();
hydrateVerificationContext();

themeToggle?.addEventListener('click', () => {
	const nextTheme = document.documentElement.dataset.theme === 'dark' ? 'light' : 'dark';
	setTheme(nextTheme);
});

searchForm?.addEventListener('submit', (event) => {
	event.preventDefault();

	const query = searchInput?.value.trim();
	const verifyUrl = searchForm.dataset.verifyUrl;
    const searchMode = searchForm.dataset.searchMode || 'verification';

	if (!query) {
		searchStatus.textContent = searchMode === 'profiles'
			? 'Enter a person, driver, owner, vehicle, fleet, or platform to search relationships.'
			: searchMode === 'opportunity'
				? 'Enter a vehicle, platform, fleet, licence type, location, or role to search opportunities.'
			: 'Enter a driver name, registration, platform, or city to start verification.';
		return;
	}

	if (!verifyUrl) {
		searchStatus.textContent = 'Verification search is not configured yet.';
		return;
	}

	searchStatus.textContent = searchMode === 'profile'
		? `Searching known relationship records for "${query}"...`
		: searchMode === 'opportunity'
			? `Searching opportunities for "${query}"...`
		: `Checking trust signals for "${query}"...`;
	loadVerification(verifyUrl, query);
});

searchModeTabs.forEach((tab) => {
	tab.addEventListener('click', () => {
		const mode = tab.dataset.searchModeTab || 'profile';

		searchModeTabs.forEach((item) => item.classList.remove('is-active'));
		tab.classList.add('is-active');

		if (searchForm) {
			searchForm.dataset.searchMode = mode;
		}

		if (opportunityControls) {
			opportunityControls.classList.toggle('is-hidden', mode !== 'opportunity');
		}

		if (profileSearchLabel) {
			profileSearchLabel.textContent = mode === 'opportunity' ? 'Find an employment or partnership opportunity' : 'Find a specific person or relationship record';
		}

		if (searchInput) {
			searchInput.placeholder = mode === 'opportunity'
				? 'Search: Code 14 driver, vehicle hire, Bolt owner, refrigerated truck'
				: 'Search: James Patrick or WWW 118 LM';
			searchInput.value = '';
		}

		if (searchStatus) {
			searchStatus.textContent = mode === 'opportunity'
				? 'Choose your intent, then search for vehicles, owners, platforms, fleets, licences, or driver roles.'
				: 'Search for a known person or relationship by name or vehicle registration.';
		}
	});
});

intentSelect?.addEventListener('change', configureRelationshipOptions);

filterChips.forEach((chip) => {
	chip.addEventListener('click', () => {
		filterChips.forEach((item) => item.classList.remove('is-active'));
		chip.classList.add('is-active');
	});
});

async function loadVerification(verifyUrl, query) {
	try {
		const params = new URLSearchParams({ query });

		if (searchForm?.dataset.searchMode) {
			params.set('mode', searchForm.dataset.searchMode);
		}

		if (intentSelect && !opportunityControls?.classList.contains('is-hidden')) {
			params.set('intent', intentSelect.selectedOptions[0]?.textContent?.trim() || intentSelect.value);
		}

		if (relationshipSelect && !opportunityControls?.classList.contains('is-hidden')) {
			params.set('relationshipType', relationshipSelect.value);
		}

		const response = await fetch(`${verifyUrl}?${params.toString()}`, {
			headers: { Accept: 'application/json' }
		});

		if (!response.ok) {
			throw new Error(`Verification returned ${response.status}`);
		}

		const dashboard = await response.json();
		renderVerification(dashboard);
	} catch (error) {
		searchStatus.textContent = 'Verification is temporarily unavailable. Try again after the API is running.';
		console.warn(error);
	}
}

function renderVerification(dashboard) {
	const drivers = dashboard.drivers || [];
	const featuredDriver = drivers[0];

	searchStatus.textContent = dashboard.apiStatus || 'Verification complete.';
	renderProfile(featuredDriver, dashboard.apiConnected);
	renderFeed(drivers);
	renderComparison(drivers);
}

function renderProfile(driver, apiConnected) {
	if (!profilePanel || !profileStatus || !profileName || !profileMeta || !profileTrust || !profileReports || !profileRisk || !profileLinks) {
		return;
	}

	if (!driver) {
		profilePanel.classList.add('is-empty');
		profileStatus.textContent = apiConnected ? 'No profile found' : 'API unavailable';
		profileName.textContent = 'No matching profile';
		profileMeta.textContent = 'Try a different name, vehicle registration, partner, platform, or region.';
		profileTrust.textContent = '--';
		profileReports.textContent = '--';
		profileRisk.textContent = '--';
		profileLinks.replaceChildren();
		return;
	}

	profilePanel.classList.remove('is-empty');
	profilePanel.dataset.userId = driver.userId || '';
	profileStatus.textContent = apiConnected ? 'Live API profile' : 'Preview profile';
	profileName.textContent = driver.name;
	profileMeta.textContent = `${driver.category} · ${driver.region} · ${driver.partnerName}`;
	profileTrust.textContent = driver.trustScore;
	profileReports.textContent = driver.reportCount;
	profileRisk.textContent = driver.riskLevel;
	profileLinks.replaceChildren(
		linkedEntity('Vehicle', `${driver.vehicleRegistration} · ${driver.vehicleDescription}`),
		linkedEntity('Partner', driver.partnerName),
		linkedEntity('Classification', driver.userType)
	);
}

function renderFeed(drivers) {
	if (!feedEmpty || !feedResults) {
		return;
	}

	feedResults.replaceChildren();
	feedEmpty.classList.toggle('is-hidden', drivers.length > 0);

	drivers.forEach((driver) => {
		feedResults.appendChild(feedResults.dataset.resultsMode === 'profiles' ? profileMatchCard(driver) : feedCard(driver));
	});
}

function profileMatchCard(driver) {
	const article = document.createElement('article');
	article.className = 'profile-match-card';

	const top = document.createElement('div');
	top.className = 'profile-match-card__top';
	const avatar = document.createElement('div');
	avatar.className = 'feed-card__avatar';
	avatar.textContent = driver.initials;
	const meta = document.createElement('div');
	meta.className = 'feed-card__meta';
	const name = document.createElement('strong');
	name.textContent = driver.name;
	const detail = document.createElement('span');
	detail.textContent = `${driver.userType} · ${driver.partnerName}`;
	meta.append(name, detail);
	top.append(avatar, meta);

	const summary = document.createElement('p');
	summary.textContent = driver.activitySummary;

	const score = document.createElement('div');
	score.className = 'profile-match-card__score';
	score.append(
		metric('Trust', driver.trustScore),
		metric('Risk', driver.riskLevel),
		metric('Vehicle', driver.vehicleRegistration)
	);

	const signals = document.createElement('div');
	signals.className = 'signal-row';
	(driver.signals || []).forEach((signal) => {
		const item = document.createElement('span');
		item.className = /pending|review/i.test(signal) ? 'signal signal--watch' : 'signal';
		item.textContent = signal;
		signals.appendChild(item);
	});

	const action = document.createElement('button');
	action.type = 'button';
	action.className = 'profile-match-card__action';
	action.textContent = 'Start relationship';
    action.dataset.profileName = driver.name;
    action.dataset.userId = driver.userId || '';
    action.dataset.trustScore = driver.trustScore;
	action.dataset.riskLevel = driver.riskLevel;
	action.dataset.vehicle = `${driver.vehicleRegistration} · ${driver.vehicleDescription}`;
	action.dataset.partner = driver.partnerName;
    action.dataset.relationshipContext = searchForm?.dataset.searchMode === 'opportunity' && relationshipSelect
        ? relationshipSelect.value
        : 'Relationship verification';

	article.append(top, summary, score, signals, action);
	return article;
}

function metric(label, value) {
	const item = document.createElement('div');
	const labelNode = document.createElement('span');
	labelNode.textContent = label;
	const valueNode = document.createElement('strong');
	valueNode.textContent = value;
	item.append(labelNode, valueNode);
	return item;
}

document.addEventListener('click', (event) => {
	const action = event.target.closest('.profile-match-card__action');

	if (!action) {
		return;
	}

	const params = new URLSearchParams({
		profile: action.dataset.profileName || '',
		profileId: action.dataset.userId || '',
		context: action.dataset.relationshipContext || 'Relationship verification',
		trust: action.dataset.trustScore || '',
		risk: action.dataset.riskLevel || '',
		vehicle: action.dataset.vehicle || '',
		partner: action.dataset.partner || ''
	});

	window.location.href = `/Verify?${params.toString()}`;
});

evidenceInputs.forEach((input) => {
	input.addEventListener('change', updateEvidenceStatus);
});

verificationForm?.addEventListener('submit', (event) => {
	event.preventDefault();

	const selectedEvidenceCount = selectedEvidenceFiles().length;
	const checkedCount = Array.from(verificationChecks).filter((item) => item.checked).length;
	const profileId = Number.parseInt(verifyProfile?.dataset.profileId || '0', 10);

	if (selectedEvidenceCount === 0) {
		verificationStatus.textContent = 'Attach at least one supporting document before submitting verification.';
		return;
	}

	if (!profileId) {
		verificationStatus.textContent = 'Start from a Relationships result before API submission so the case has a linked profile id.';
		return;
	}

	submitVerificationCase(profileId, selectedEvidenceCount, checkedCount);
});

signalDraft?.addEventListener('click', () => {
	const context = signalContext?.value.trim() || '';
	updateSignalStatus(
		context
			? 'Draft signal saved locally for review. Backend moderation submission is pending.'
			: 'Add context before saving a useful moderation draft.',
		context ? 'success' : 'warning'
	);
});

signalForm?.addEventListener('submit', (event) => {
	event.preventDefault();

	const context = signalContext?.value.trim() || '';

	if (context.length < 20) {
		updateSignalStatus('Add at least 20 characters of context so moderators can review the signal fairly.', 'warning');
		signalContext?.focus();
		return;
	}

	updateSignalStatus(
		'Submitting signal to moderation...',
		'neutral'
	);
	submitFeedbackSignal(context);
});

function updateSignalStatus(message, tone) {
	if (!signalStatus) {
		return;
	}

	signalStatus.textContent = message;
	signalStatus.classList.toggle('is-success', tone === 'success');
	signalStatus.classList.toggle('is-warning', tone === 'warning');
}

function hydrateVerificationContext() {
	if (!verificationForm) {
		return;
	}

	const params = new URLSearchParams(window.location.search);

	if (verifyProfile && params.has('profile')) {
		verifyProfile.value = params.get('profile') || '';
		verifyProfile.dataset.profileId = params.get('profileId') || '';
	}

	if (verifyContext && params.has('context')) {
		verifyContext.value = params.get('context') || '';
	}

	hydrateTrustCue(params);
}

function hydrateTrustCue(params) {
	if (!trustCueScore || !trustCueName || !trustCueSummary || !trustCueRisk || !trustCueLinks) {
		return;
	}

	const profile = params.get('profile');
	const trust = params.get('trust');
	const risk = params.get('risk');
	const vehicle = params.get('vehicle');
	const partner = params.get('partner');
	const context = params.get('context');

	if (!profile) {
		return;
	}

	trustCueScore.textContent = trust || '--';
	trustCueName.textContent = profile;
	trustCueRisk.textContent = risk || 'Review';
	trustCueSummary.textContent = `${profile} is being reviewed for ${context || 'a relationship verification'}. Use the trust score and linked entities as a cue before submitting evidence.`;
	trustCueLinks.replaceChildren(
		linkedEntity('Risk', risk || 'Review'),
		linkedEntity('Vehicle', vehicle || 'Vehicle pending'),
		linkedEntity('Partner', partner || 'Partner pending')
	);
}

function renderComparison(drivers) {
	if (!profileComparison || !comparisonResults || !searchForm) {
		return;
	}

	const showComparison = searchForm.dataset.searchMode === 'opportunity' && drivers.length > 0;
	profileComparison.classList.toggle('is-hidden', !showComparison);
	comparisonResults.replaceChildren();

	if (!showComparison) {
		return;
	}

	[...drivers]
		.sort((first, second) => second.trustScore - first.trustScore)
		.forEach((driver, index) => {
			comparisonResults.appendChild(candidateRow(driver, index + 1));
		});
}

function candidateRow(driver, rank) {
	const row = document.createElement('div');
	row.className = 'candidate-row';

	const name = document.createElement('span');
	name.textContent = `${rank}. ${driver.name}`;
	const score = document.createElement('strong');
	score.textContent = driver.trustScore;
	const detail = document.createElement('small');
	detail.textContent = `${driver.userType} · ${driver.riskLevel} risk · ${driver.vehicleRegistration}`;

	row.append(name, score, detail);
	return row;
}

function updateEvidenceStatus() {
	if (!evidenceStatus) {
		return;
	}

	const files = selectedEvidenceFiles();
	evidenceStatus.textContent = files.length === 0
		? 'No documents selected yet.'
		: `${files.length} document${files.length === 1 ? '' : 's'} selected: ${files.join(', ')}`;
}

function selectedEvidenceFiles() {
	return Array.from(evidenceInputs)
		.flatMap((input) => Array.from(input.files || []))
		.map((file) => file.name);
}

function feedCard(driver) {
	const article = document.createElement('article');
	article.className = `feed-card ${driver.riskLevel === 'Review' ? 'feed-card--alert' : ''}`;

	const avatar = document.createElement('div');
	avatar.className = 'feed-card__avatar';
	avatar.textContent = driver.initials;

	const body = document.createElement('div');
	body.className = 'feed-card__body';

	const meta = document.createElement('div');
	meta.className = 'feed-card__meta';
	const name = document.createElement('strong');
	name.textContent = driver.name;
	const detail = document.createElement('span');
	detail.textContent = `${driver.category} · ${driver.region} · ${driver.partnerName}`;
	meta.append(name, detail);

	const summary = document.createElement('p');
	summary.textContent = driver.activitySummary;

	const signals = document.createElement('div');
	signals.className = 'signal-row';
	(driver.signals || []).forEach((signal) => {
		const item = document.createElement('span');
		item.className = /pending|review/i.test(signal) ? 'signal signal--watch' : 'signal';
		item.textContent = signal;
		signals.appendChild(item);
	});

	body.append(meta, summary, signals);
	article.append(avatar, body);

	return article;
}

function linkedEntity(label, value) {
	const item = document.createElement('div');
	item.className = 'linked-entity';
	const labelNode = document.createElement('span');
	labelNode.textContent = label;
	const valueNode = document.createElement('strong');
	valueNode.textContent = value;
	item.append(labelNode, valueNode);
	return item;
}

function configureRelationshipOptions() {
	if (!intentSelect || !relationshipSelect) {
		return;
	}

	const relationshipsByIntent = {
		'looking-for-driver': ['Employment', 'Vehicle hire'],
		'driver-looking-for-owner': ['Vehicle hire', 'Fleet contract'],
		'fleet-owner-looking-for-partners': ['Fleet contract', 'Delivery partnership'],
		'platform-vetting-profiles': ['Employment', 'Fleet contract', 'Delivery partnership']
	};

	const options = relationshipsByIntent[intentSelect.value] || relationshipsByIntent['looking-for-driver'];
	relationshipSelect.replaceChildren(...options.map((label) => {
		const option = document.createElement('option');
		option.value = label;
		option.textContent = label;
		return option;
	}));
}
