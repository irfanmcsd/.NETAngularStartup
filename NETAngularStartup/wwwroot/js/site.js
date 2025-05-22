/*
function generateCategoryMenu(categories, parentElement) {
    const ul = document.createElement('ul');
    ul.className = 'navbar-nav flex-column w-100';

    categories.forEach(category => {
        const li = document.createElement('li');
        li.className = 'nav-item';

        const hasChildren = category.Children && category.Children.length > 0;

        const link = document.createElement('a');
        link.className = hasChildren ? 'nav-link collapsed' : 'nav-link';
        link.href = `/categories/${category.Id}`;
        link.textContent = category.Name;

        if (hasChildren) {
            link.setAttribute('data-bs-toggle', 'collapse');
            link.setAttribute('data-bs-target', `#submenu-${category.Id}`);
            link.setAttribute('aria-expanded', 'false');
            link.setAttribute('aria-controls', `submenu-${category.Id}`);

            const div = document.createElement('div');
            div.id = `submenu-${category.Id}`;
            div.className = 'collapse children';

            generateCategoryMenu(category.Children, div);

            li.appendChild(link);
            li.appendChild(div);
        } else {
            li.appendChild(link);
        }

        ul.appendChild(li);
    });

    parentElement.appendChild(ul);
}

// Usage when you have your data:
document.addEventListener('DOMContentLoaded', () => {
    const navContainer = document.getElementById('categoryNav');
    // Assuming you have your hierarchy data in a variable:
    // const categoryHierarchy = ... (from your API call)
    generateCategoryMenu(categoryHierarchy, navContainer);
});*/