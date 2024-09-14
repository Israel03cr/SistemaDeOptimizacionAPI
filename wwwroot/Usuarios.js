const apiUrl = 'http://localhost:5298/api/Account'; // Asegúrate de que la URL sea correcta
let editingUserId = null; // Para saber si estamos editando un usuario

// Obtener todos los usuarios al cargar la página
window.onload = function() {
    fetchUsers();
};

// Función para obtener todos los usuarios
function fetchUsers() {
    fetch(`${apiUrl}/getAllUsers`)
        .then(response => response.json())
        .then(data => {
            const usersList = document.getElementById('usersList');
            usersList.innerHTML = ''; // Limpiar la lista
            data.forEach(user => {
                usersList.innerHTML += `
                    <tr>
                        <td>${user.id}</td>
                        <td>${user.nombre}</td>
                        <td>${user.apellido}</td>
                        <td>${user.email}</td>
                        <td>${user.direccion}</td>
                        <td>${user.telefono}</td>
                        <td>
                            <button class="btn btn-warning" onclick="editUser('${user.id}')">Editar</button>
                            <button class="btn btn-danger" onclick="deleteUser('${user.id}')">Eliminar</button>
                        </td>
                    </tr>
                `;
            });
        })
        .catch(error => console.error('Error al obtener usuarios:', error));
}

// Registrar nuevo usuario
document.getElementById('registerForm').addEventListener('submit', function(event) {
    event.preventDefault();

    const data = {
        Nombre: document.getElementById('nombre').value,
        Apellido: document.getElementById('apellido').value,
        Email: document.getElementById('email').value,
        Password: document.getElementById('password').value,
        Direccion: document.getElementById('direccion').value,
        Telefono: document.getElementById('telefono').value
    };

    fetch(`${apiUrl}/register`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    })
    .then(response => {
        if (response.ok) {
            document.getElementById('registerResult').innerHTML = '<div class="alert alert-success">Registro exitoso</div>';
            resetForm();
            fetchUsers(); // Actualiza la lista de usuarios
        } else {
            response.text().then(text => {
                document.getElementById('registerResult').innerHTML = '<div class="alert alert-danger">Error: ' + text + '</div>';
            });
        }
    })
    .catch(error => {
        document.getElementById('registerResult').innerHTML = '<div class="alert alert-danger">Error: ' + error + '</div>';
    });
});

// Función para cargar datos del usuario en el formulario para editar
function editUser(userId) {
    fetch(`${apiUrl}/getAllUsers`)
        .then(response => response.json())
        .then(users => {
            const user = users.find(u => u.id === userId);
            document.getElementById('nombre').value = user.nombre;
            document.getElementById('apellido').value = user.apellido;
            document.getElementById('email').value = user.email;
            document.getElementById('direccion').value = user.direccion;
            document.getElementById('telefono').value = user.telefono;
            editingUserId = user.id;

            document.getElementById('updateButton').classList.remove('d-none');
            document.getElementById('cancelButton').classList.remove('d-none');
            document.getElementById('registerForm').querySelector('button[type="submit"]').classList.add('d-none');
        })
        .catch(error => console.error('Error al cargar los datos del usuario:', error));
}

// Actualizar usuario
document.getElementById('updateButton').addEventListener('click', function(event) {
    event.preventDefault();

    const data = {
        UserId: editingUserId,
        Nombre: document.getElementById('nombre').value,
        Apellido: document.getElementById('apellido').value,
        Email: document.getElementById('email').value,
        Direccion: document.getElementById('direccion').value,
        Telefono: document.getElementById('telefono').value
    };

    fetch(`${apiUrl}/updateUser`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    })
    .then(response => {
        if (response.ok) {
            document.getElementById('registerResult').innerHTML = '<div class="alert alert-success">Usuario actualizado con éxito</div>';
            resetForm();
            fetchUsers(); // Actualiza la lista de usuarios
        } else {
            response.text().then(text => {
                document.getElementById('registerResult').innerHTML = '<div class="alert alert-danger">Error: ' + text + '</div>';
            });
        }
    })
    .catch(error => {
        document.getElementById('registerResult').innerHTML = '<div class="alert alert-danger">Error: ' + error + '</div>';
    });
});

// Eliminar usuario
function deleteUser(userId) {
    if (confirm('¿Estás seguro de que deseas eliminar este usuario?')) {
        fetch(`${apiUrl}/deleteUser/${userId}`, {
            method: 'DELETE'
        })
        .then(response => {
            if (response.ok) {
                fetchUsers(); // Actualiza la lista de usuarios
                document.getElementById('registerResult').innerHTML = '<div class="alert alert-success">Usuario eliminado con éxito</div>';
            } else {
                document.getElementById('registerResult').innerHTML = '<div class="alert alert-danger">Error al eliminar el usuario</div>';
            }
        })
        .catch(error => {
            document.getElementById('registerResult').innerHTML = '<div class="alert alert-danger">Error: ' + error + '</div>';
        });
    }
}

// Función para resetear el formulario
function resetForm() {
    document.getElementById('registerForm').reset();
    editingUserId = null;

    document.getElementById('updateButton').classList.add('d-none');
    document.getElementById('cancelButton').classList.add('d-none');
    document.getElementById('registerForm').querySelector('button[type="submit"]').classList.remove('d-none');
}

// Cancelar edición
document.getElementById('cancelButton').addEventListener('click', function() {
    resetForm();
});
