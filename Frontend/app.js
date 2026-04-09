/**
 * Professional Career Portal - Core SPA Engine
 * Palette: Mystic Jade (#77A8A8) & Pale White (#F7E7CE)
 */

const API_BASE = 'http://localhost:5178/api';

const state = {
    user: null,
    role: null,
    token: localStorage.getItem('token'),
    currentView: 'login'
};

// --- CORE UTILS ---

function parseJwt(token) {
    if (!token) return null;
    try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(atob(base64).split('').map(c => {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join(''));
        return JSON.parse(jsonPayload);
    } catch (e) {
        return null;
    }
}

function showToast(message, type = 'info') {
    const container = document.getElementById('toast-container');
    const toast = document.createElement('div');
    toast.className = `toast ${type}`;
    toast.innerHTML = `<span>${message}</span>`;
    container.appendChild(toast);
    setTimeout(() => {
        toast.style.opacity = '0';
        toast.style.transform = 'translateX(20px)';
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}

// --- AUTH LOGIC ---

async function login(email, password) {
    try {
        const res = await fetch(`${API_BASE}/Account/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password })
        });
        const data = await res.json();
        if (res.ok) {
            localStorage.setItem('token', data.token);
            localStorage.setItem('email', data.email);
            state.token = data.token;
            showToast('Welcome back!', 'success');
            initApp();
        } else {
            showToast(data.message || 'Login failed', 'error');
        }
    } catch (err) {
        showToast('Connection failed. Backend might be offline.', 'error');
    }
}

async function register(email, password, role) {
    try {
        const res = await fetch(`${API_BASE}/Account/register`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password, role })
        });
        if (res.ok) {
            showToast('Registration successful! You can now login.', 'success');
            navigateTo('login');
        } else {
            const data = await res.json();
            showToast(data.message || 'Registration failed', 'error');
        }
    } catch (err) {
        showToast('Network error during registration.', 'error');
    }
}

function logout() {
    localStorage.clear();
    state.user = null;
    state.role = null;
    state.token = null;
    document.getElementById('main-header').style.display = 'none';
    navigateTo('login');
}

// --- VIEW MANAGER ---

function renderNav() {
    const nav = document.getElementById('top-nav');
    if (state.role === 'Admin') {
        nav.innerHTML = `
            <a href="#" class="nav-link" onclick="navigateTo('admin')">Dashboard</a>
            <a href="#" class="nav-link" onclick="showToast('Registry coming soon', 'info')">Registry</a>
        `;
    } else if (state.role === 'Employer') {
        nav.innerHTML = `
            <a href="#" class="nav-link" onclick="navigateTo('recruiter')">Recruitment Hub</a>
            <a href="#" class="nav-link" onclick="showToast('Course Manager coming soon', 'info')">Courses</a>
        `;
    } else {
        nav.innerHTML = `
            <a href="#" class="nav-link" onclick="navigateTo('student')">My Hub</a>
            <a href="#" class="nav-link" onclick="showToast('Applications view coming soon', 'info')">Applications</a>
        `;
    }
}

const views = {
    login: () => `
        <div class="auth-wrapper animate-fade-in">
            <div class="glass-card auth-card">
                <center><div class="logo-icon" style="font-size: 3rem; margin-bottom: 1rem;">✦</div></center>
                <h2 class="text-center mb-4">Portal Login</h2>
                <div class="form-group">
                    <label>Email Address</label>
                    <input type="email" id="login-email" placeholder="name@domain.com">
                </div>
                <div class="form-group">
                    <label>Password</label>
                    <input type="password" id="login-password" placeholder="••••••••">
                </div>
                <button class="btn btn-primary w-full" style="width: 100%;" onclick="handleLoginSubmit()">Sign In</button>
                <p class="text-center mt-4 text-sm" style="color: var(--text-muted);">
                    New here? <a href="#" onclick="navigateTo('register')" style="color: var(--primary); font-weight: 600;">Create Account</a>
                </p>
            </div>
        </div>
    `,
    register: () => `
        <div class="auth-wrapper animate-fade-in">
            <div class="glass-card auth-card">
                <h2 class="text-center mb-4">Digital Enrollment</h2>
                <div class="form-group">
                    <label>Email Address</label>
                    <input type="email" id="reg-email" placeholder="name@domain.com">
                </div>
                <div class="form-group">
                    <label>Password</label>
                    <input type="password" id="reg-password" placeholder="Min 8 characters">
                </div>
                <div class="form-group">
                    <label>Your Role</label>
                    <select id="reg-role">
                        <option value="Student">Professional Student</option>
                        <option value="Employer">Portal Recruiter</option>
                    </select>
                </div>
                <button class="btn btn-primary w-full" style="width: 100%;" onclick="handleRegisterSubmit()">Join Platform</button>
                <p class="text-center mt-4 text-sm" style="color: var(--text-muted);">
                    Already joined? <a href="#" onclick="navigateTo('login')" style="color: var(--primary); font-weight: 600;">Sign In</a>
                </p>
            </div>
        </div>
    `,
    student: async () => {
        const [depts, courses, jobs] = await Promise.all([
            fetchWithAuth('/Departments'),
            fetchWithAuth('/Courses'),
            fetchWithAuth('/Jobs')
        ]);

        return `
            <div class="container animate-fade-in">
                <header class="mb-4">
                    <h2 style="font-size: 1.75rem;">Academic & Career Hub</h2>
                    <p style="color: var(--text-muted);">Your personalized roadmap for professional growth.</p>
                </header>

                <div class="dashboard-grid">
                    <section class="span-wide">
                        <div class="glass-card mb-4" style="background: var(--primary-light);">
                            <h4 class="mb-2">Academic Information</h4>
                            <div style="display: flex; gap: 2rem; flex-wrap: wrap;">
                                ${depts.map(d => `
                                    <div class="text-sm">
                                        <b>Department:</b> ${d.name} <br>
                                        <span style="color: var(--text-muted);">Location: ${d.location}</span>
                                    </div>
                                `).join('')}
                            </div>
                        </div>
                    </section>

                    <section>
                        <h3 class="mb-4">Developmental Courses</h3>
                        <div class="course-list">
                            ${courses.length ? courses.map(c => `
                                <div class="glass-card item-card mb-4" style="padding: 1.5rem;">
                                    <div class="item-header">
                                        <h4>${c.title}</h4>
                                        <span class="item-badge">${c.level || 'General'}</span>
                                    </div>
                                    <p class="text-sm" style="color: var(--text-muted);">${c.description || 'Enhance your skills with this course.'}</p>
                                    <div style="display: flex; justify-content: space-between; align-items: center; margin-top: 1rem;">
                                        <span class="text-sm"><b>Duration:</b> ${c.duration || 'Flexible'}</span>
                                        <span class="text-sm" style="color: var(--primary); font-weight: 700;">${c.price > 0 ? `$${c.price}` : 'FREE'}</span>
                                    </div>
                                    <button class="btn btn-outline btn-sm mt-2" onclick="showToast('Enrollment feature coming soon', 'info')">View Syllabus</button>
                                </div>
                            `).join('') : '<p>No courses available.</p>'}
                        </div>
                    </section>

                    <section>
                        <h3 class="mb-4">Career Opportunities</h3>
                        <div class="job-list">
                            ${jobs.length ? jobs.map(j => `
                                <div class="glass-card item-card mb-4" style="padding: 1.5rem;">
                                    <div class="item-header">
                                        <h4>${j.title}</h4>
                                        <span class="item-badge" style="background: rgba(129, 178, 154, 0.1); color: #81B29A;">ACTIVE</span>
                                    </div>
                                    <p class="text-sm" style="color: var(--text-muted);">${j.description}</p>
                                    
                                    ${j.recommendedCourses.length ? `
                                        <div class="mt-4">
                                            <p class="text-xs font-bold" style="text-transform: uppercase; color: var(--primary);">Upskilling Recommendations:</p>
                                            <div style="display: flex; flex-wrap: wrap; gap: 0.25rem; margin-top: 0.25rem;">
                                                ${j.recommendedCourses.map(rc => `<span class="item-badge" style="font-size: 0.6rem;">${rc.title}</span>`).join('')}
                                            </div>
                                        </div>
                                    ` : ''}

                                    <button class="btn btn-primary mt-4" style="width: 100%;" onclick="applyToJob(${j.id})">One-Click Apply</button>
                                </div>
                            `).join('') : '<p>No open vacancies found.</p>'}
                        </div>
                    </section>
                </div>
            </div>
        `;
    },
    recruiter: async () => {
        const apps = await fetchWithAuth('/Jobs/applications');
        const jobs = await fetchWithAuth('/Jobs');
        const courses = await fetchWithAuth('/Courses');
        const userId = localStorage.getItem('email'); // Fallback identifier

        // Filter work by this employer (if logic is server-side, this just visualizes)
        const myJobs = jobs.filter(j => j.employerName === userId || j.employerId === state.user?.id);
        const myCourses = courses.filter(c => c.instructorName === userId || c.instructorId === state.user?.id);

        return `
            <div class="container animate-fade-in">
                <header class="mb-4" style="display: flex; justify-content: space-between; align-items: flex-end;">
                    <div>
                        <h2 style="font-size: 1.75rem;">Talent & Learning Console</h2>
                        <p style="color: var(--text-muted);">Coordinate your hiring pipeline and upskilling catalog.</p>
                    </div>
                    <div style="display: flex; gap: 0.5rem;">
                        <button class="btn btn-outline" onclick="showCourseForm()">+ Add Course</button>
                        <button class="btn btn-primary" onclick="showJobForm()">+ Post Vacancy</button>
                    </div>
                </header>

                <div class="dashboard-grid">
                    <section>
                        <h3 class="mb-4">Internal Course Catalog</h3>
                        <div id="employer-course-list">
                            ${courses.map(c => `
                                <div class="glass-card mb-4" style="padding: 1rem;">
                                    <div style="display: flex; justify-content: space-between;">
                                        <h5>${c.title}</h5>
                                        <span class="item-badge">${c.level}</span>
                                    </div>
                                    <p class="text-sm mt-2">${c.description.substring(0, 60)}...</p>
                                    <div class="mt-4" style="display: flex; gap: 0.5rem;">
                                        <button class="btn btn-outline btn-sm" onclick="editCourse(${c.courseID})">Edit</button>
                                        <button class="btn btn-icon btn-sm" onclick="deleteCourse(${c.courseID})" style="color: var(--error);">🗑</button>
                                    </div>
                                </div>
                            `).join('')}
                        </div>
                    </section>

                    <section>
                        <h3 class="mb-4">Active Vacancies & Recommendations</h3>
                        <div id="employer-job-list">
                            ${jobs.map(j => `
                                <div class="glass-card mb-4" style="padding: 1.5rem;">
                                    <div class="item-header">
                                        <h4>${j.title}</h4>
                                        <div style="display: flex; gap: 0.5rem;">
                                            <button class="btn btn-outline btn-sm" onclick="showLinkModal(${j.id})">🔗 Link Courses</button>
                                        </div>
                                    </div>
                                    <div class="mt-2 text-sm">
                                        <b>Recommended Upskilling:</b> 
                                        ${j.recommendedCourses.length ? j.recommendedCourses.map(rc => `<span class="item-badge" style="margin-right: 4px;">${rc.title}</span>`).join('') : '<span style="color: var(--text-muted);">None linked</span>'}
                                    </div>
                                    <button class="btn btn-primary mt-4 w-full" style="width: 100%;" onclick="showToast('Loading applicant manager...', 'info')">Manage Applicants</button>
                                </div>
                            `).join('')}
                        </div>
                    </section>
                </div>
            </div>

            <!-- Simple Modal Shadow -->
            <div id="modal-overlay" class="loader-wrapper" style="display:none; background: rgba(0,0,0,0.4);">
                <div class="glass-card" style="width: 100%; max-width: 500px; background: white;">
                    <div id="modal-content"></div>
                    <div class="mt-4" style="display: flex; justify-content: flex-end; gap: 1rem;">
                        <button class="btn btn-outline" onclick="closeModal()">Cancel</button>
                        <button class="btn btn-primary" id="modal-confirm-btn">Confirm</button>
                    </div>
                </div>
            </div>
        `;
    },
    admin: async () => {
        const stats = await fetchWithAuth('/Admin/stats');
        return `
            <div class="container animate-fade-in">
                <header class="mb-4">
                    <h2 style="font-size: 1.75rem;">System Administration</h2>
                    <p style="color: var(--text-muted);">Global oversight of portal activity and metrics.</p>
                </header>

                <div class="dashboard-grid">
                    <div class="glass-card stats-card">
                        <span class="label">Total Students</span>
                        <span class="value">${stats.totalStudents}</span>
                    </div>
                    <div class="glass-card stats-card">
                        <span class="label">Job Vacancies</span>
                        <span class="value">${stats.totalJobs}</span>
                    </div>
                    <div class="glass-card stats-card">
                        <span class="label">Applications</span>
                        <span class="value">${stats.totalApplications}</span>
                    </div>
                    <div class="glass-card stats-card">
                        <span class="label">Departments</span>
                        <span class="value">${stats.totalDepartments}</span>
                    </div>
                </div>

                <div class="mt-4">
                    <h3 class="mb-4">Resource Management</h3>
                    <div class="glass-card" style="padding: 3rem; text-align: center;">
                        <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="var(--primary)" stroke-width="2"><rect x="3" y="3" width="18" height="18" rx="2" ry="2"/><line x1="3" y1="9" x2="21" y2="9"/><line x1="9" y1="21" x2="9" y2="9"/></svg>
                        <h4 class="mt-2">Entity Registry</h4>
                        <p class="text-sm mt-2" style="color: var(--text-muted);">Manage students, teachers, and academic requirements from the mission control interface.</p>
                        <button class="btn btn-outline mt-4" onclick="showToast('Admin management view loading...', 'info')">Enter Registry</button>
                    </div>
                </div>
            </div>
        `;
    }
};

// --- MODAL UTILS ---

function showModal(html, onConfirm) {
    const overlay = document.getElementById('modal-overlay');
    const content = document.getElementById('modal-content');
    const confirmBtn = document.getElementById('modal-confirm-btn');
    
    content.innerHTML = html;
    confirmBtn.onclick = onConfirm;
    overlay.style.display = 'flex';
}

function closeModal() {
    document.getElementById('modal-overlay').style.display = 'none';
}

// --- EMPLOYER ACTIONS ---

function showCourseForm(courseData = null) {
    const html = `
        <h3 class="mb-4">${courseData ? 'Edit' : 'New'} Developmental Course</h3>
        <div class="form-group">
            <label>Course Title</label>
            <input type="text" id="c-title" value="${courseData?.title || ''}">
        </div>
        <div class="form-group">
            <label>Description</label>
            <textarea id="c-desc">${courseData?.description || ''}</textarea>
        </div>
        <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 1rem;">
            <div class="form-group">
                <label>Duration</label>
                <input type="text" id="c-dur" value="${courseData?.duration || '8 Weeks'}">
            </div>
            <div class="form-group">
                <label>Level</label>
                <select id="c-lvl">
                    <option ${courseData?.level === 'Beginner' ? 'selected' : ''}>Beginner</option>
                    <option ${courseData?.level === 'Intermediate' ? 'selected' : ''}>Intermediate</option>
                    <option ${courseData?.level === 'Advanced' ? 'selected' : ''}>Advanced</option>
                </select>
            </div>
        </div>
        <div class="form-group">
            <label>Price ($)</label>
            <input type="number" id="c-price" value="${courseData?.price || 0}">
        </div>
    `;
    
    showModal(html, async () => {
        const dto = {
            title: document.getElementById('c-title').value,
            description: document.getElementById('c-desc').value,
            duration: document.getElementById('c-dur').value,
            level: document.getElementById('c-lvl').value,
            price: parseFloat(document.getElementById('c-price').value),
            credits: 4,
            deptID: 1 // CS by default for demo
        };
        
        const method = courseData ? 'PUT' : 'POST';
        const url = courseData ? `/Courses/${courseData.courseID}` : '/Courses';
        
        try {
            const res = await fetch(`${API_BASE}${url}`, {
                method,
                headers: { 
                    'Authorization': `Bearer ${state.token}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(dto)
            });
            if (res.ok) {
                showToast(courseData ? 'Course updated' : 'Course created', 'success');
                closeModal();
                navigateTo('recruiter');
            } else {
                showToast('Failed to save course', 'error');
            }
        } catch(e) { showToast('Network error', 'error'); }
    });
}

async function showLinkModal(jobId) {
    const courses = await fetchWithAuth('/Courses');
    const job = await fetchWithAuth(`/Jobs/${jobId}`);
    const linkedIds = job.recommendedCourses.map(c => c.courseId);

    const html = `
        <h3 class="mb-4">Link Upskilling Recommendations</h3>
        <p class="text-sm mb-4">Select courses that help students qualify for this role.</p>
        <div style="max-height: 300px; overflow-y: auto;">
            ${courses.map(c => `
                <div style="display: flex; align-items: center; gap: 0.75rem; padding: 0.5rem; border-bottom: 1px solid var(--border);">
                    <input type="checkbox" id="link-${c.courseID}" ${linkedIds.includes(c.courseID) ? 'checked' : ''} style="width: auto;">
                    <label for="link-${c.courseID}" style="margin: 0;">${c.title} <small>(${c.level})</small></label>
                </div>
            `).join('')}
        </div>
    `;

    showModal(html, async () => {
        const checkboxes = document.querySelectorAll('[id^="link-"]');
        for (const box of checkboxes) {
            const courseId = parseInt(box.id.split('-')[1]);
            const isChecked = box.checked;
            const wasChecked = linkedIds.includes(courseId);

            if (isChecked && !wasChecked) {
                await fetch(`${API_BASE}/Jobs/${jobId}/LinkCourse/${courseId}`, {
                    method: 'POST',
                    headers: { 'Authorization': `Bearer ${state.token}` }
                });
            } else if (!isChecked && wasChecked) {
                // DELETE logic placeholder – the endpoint doesn't exist yet, but for demo we show logic flow
                showToast('Unlinking is managed by system admins', 'info');
            }
        }
        showToast('Associations updated', 'success');
        closeModal();
        navigateTo('recruiter');
    });
}

function showJobForm() {
    showToast('Job creation window loading...', 'info');
}

async function deleteCourse(id) {
    if (!confirm('Are you sure you want to deactivate this course?')) return;
    try {
        const res = await fetch(`${API_BASE}/Courses/${id}`, {
            method: 'DELETE',
            headers: { 'Authorization': `Bearer ${state.token}` }
        });
        if (res.ok) {
            showToast('Course deactivated', 'success');
            navigateTo('recruiter');
        } else {
            showToast('Deactivation failed', 'error');
        }
    } catch(e) { showToast('Network error', 'error'); }
}

async function editCourse(id) {
    const course = await fetchWithAuth(`/Courses/${id}`);
    showCourseForm(course);
}


// --- API WRAPPER ---

async function fetchWithAuth(endpoint, options = {}) {
    const token = state.token || localStorage.getItem('token');
    const headers = {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
        ...options.headers
    };
    try {
        const res = await fetch(`${API_BASE}${endpoint}`, { ...options, headers });
        if (res.status === 401) {
            logout();
            return null;
        }
        return await res.json();
    } catch (e) {
        console.error('Fetch error:', e);
        return [];
    }
}

// --- SUBMIT HANDLERS ---

async function handleLoginSubmit() {
    const email = document.getElementById('login-email').value;
    const password = document.getElementById('login-password').value;
    if (!email || !password) return showToast('Fill all fields', 'error');
    await login(email, password);
}

async function handleRegisterSubmit() {
    const email = document.getElementById('reg-email').value;
    const password = document.getElementById('reg-password').value;
    const role = document.getElementById('reg-role').value;
    if (!email || !password) return showToast('Fill all fields', 'error');
    await register(email, password, role);
}

async function applyToJob(jobId) {
    try {
        const res = await fetch(`${API_BASE}/Jobs/${jobId}/Apply`, {
            method: 'POST',
            headers: { 
                'Authorization': `Bearer ${state.token}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ coverLetter: "I'm interested!", resumeLink: "http://example.com/resume" })
        });
        if (res.ok) {
            showToast('Application sent successfully!', 'success');
        } else {
            const data = await res.json();
            showToast(data.message || 'Already applied or error occurred', 'info');
        }
    } catch (e) {
        showToast('Application failed', 'error');
    }
}

async function updateAppStatus(appId, status) {
    try {
        const res = await fetch(`${API_BASE}/Jobs/Applications/${appId}/Status`, {
            method: 'PATCH',
            headers: { 
                'Authorization': `Bearer ${state.token}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ status })
        });
        if (res.ok) {
            showToast(`Application ${status}`, 'success');
            navigateTo('recruiter');
        } else {
            showToast('Update failed', 'error');
        }
    } catch (e) {
        showToast('Network error', 'error');
    }
}

// --- INITIALIZATION & ROUTING ---

async function navigateTo(viewName) {
    const root = document.getElementById('app-root');
    const loader = document.getElementById('global-loader');
    
    // Show loader
    if (loader) loader.style.display = 'flex';
    
    try {
        const content = await views[viewName]();
        root.innerHTML = content;
        state.currentView = viewName;
    } catch (e) {
        console.error('View error:', e);
        root.innerHTML = '<div class="container"><p class="text-error">Error loading view.</p></div>';
    } finally {
        if (loader) loader.style.display = 'none';
    }
}

async function initApp() {
    const token = localStorage.getItem('token');
    if (!token) {
        navigateTo('login');
        return;
    }

    const payload = parseJwt(token);
    if (!payload || (payload.exp * 1000 < Date.now())) {
        logout();
        return;
    }

    state.token = token;
    state.role = payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || payload.role;
    const email = payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"] || payload.email;

    // Show header
    document.getElementById('main-header').style.display = 'block';
    document.getElementById('user-display-email').textContent = `${email} (${state.role})`;
    renderNav();

    // Route based on role
    if (state.role === 'Admin') navigateTo('admin');
    else if (state.role === 'Employer') navigateTo('recruiter');
    else navigateTo('student');
}

// Start
document.addEventListener('DOMContentLoaded', initApp);
