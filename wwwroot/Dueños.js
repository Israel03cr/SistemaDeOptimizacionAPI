const apiUrl = 'http://localhost:5298/api/Dueños'; // Reemplaza con la URL correcta de tu API
let editingDueñoId = null; // Para saber si estamos editando un dueño

// Obtener todos los dueños al cargar la página
window.onload = function() {
    fetchDueños();
};

// Función para obtener todos los dueños
function fetchDueños(pageNumber = 1, pageSize = 10) {
    fetch(`${apiUrl}?pageNumber=${pageNumber}&pageSize=${pageSize}`)
        .then(response => response.json())
        .then(data => {
            const dueñosList = document.getElementById('dueñosList');
            dueñosList.innerHTML = '';
            data.forEach(dueño => {
                dueñosList.innerHTML += `
                    <tr>
                        <td>${dueño.dueñoID}</td>
                        <td>${dueño.nombre}</td>
                        <td>${dueño.apellido}</td>
                        <td>${dueño.direccion}</td>
                        <td>${dueño.telefono}</td>
                        <td>${dueño.email}</td>
                       <td class="text-center">
                                <button class="btn btn-warning" onclick="editDueño(${dueño.dueñoID})">
                                     <i class="bi bi-pencil-square"></i> 
                               </button>
                                     <button class="btn btn-danger" onclick="deleteDueño(${dueño.dueñoID})">
                                    <i class="bi bi-trash"></i>
                                   </button>
                              </td>

                    </tr>
                `;
            });
        });
}


// Registrar nuevo dueño
// Registrar nuevo dueño
document.getElementById('registerButton').addEventListener('click', function(event) {
    // Primero validar los campos del formulario
    const nombre = document.getElementById('nombre').value.trim();
    const apellido = document.getElementById('apellido').value.trim();
    const direccion = document.getElementById('direccion').value.trim();
    const telefono = document.getElementById('telefono').value.trim();
    const email = document.getElementById('email').value.trim();

    // Validar que todos los campos estén llenos
    if (!nombre || !apellido || !direccion || !telefono || !email) {
        // Si algún campo está vacío, mostrar un mensaje de error
        $('#registerResult').html('<div class="alert alert-danger">Por favor, completa todos los campos.</div>');
        return;  // Detener el envío si hay errores
    }

    const dueño = {
        nombre: nombre,
        apellido: apellido,
        direccion: direccion,
        telefono: telefono,
        email: email
    };

    fetch(apiUrl, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(dueño)
    }).then(response => {
        if (response.ok) {
            resetForm();
            fetchDueños();
            $('#registerResult').html('<div class="alert alert-success">Dueño registrado con éxito.</div>');
        } else {
            $('#registerResult').html('<div class="alert alert-danger">Error al registrar el dueño.</div>');
        }
    });
});

// Función para cargar los datos del dueño en el formulario para editar
function editDueño(id) {
    fetch(`${apiUrl}/${id}`)
        .then(response => response.json())
        .then(dueño => {
            document.getElementById('nombre').value = dueño.nombre;
            document.getElementById('apellido').value = dueño.apellido;
            document.getElementById('direccion').value = dueño.direccion;
            document.getElementById('telefono').value = dueño.telefono;
            document.getElementById('email').value = dueño.email;
            editingDueñoId = dueño.dueñoID;

            // Cambiar botones
            document.getElementById('registerButton').classList.add('d-none');  // Ocultar botón de registro
            document.getElementById('updateButton').classList.remove('d-none'); // Mostrar botón de actualizar
            document.getElementById('cancelButton').classList.remove('d-none'); // Mostrar botón de cancelar
        });
}

// Actualizar dueño
document.getElementById('updateButton').addEventListener('click', function(event) {
    event.preventDefault();

    const nombre = document.getElementById('nombre').value.trim();
    const apellido = document.getElementById('apellido').value.trim();
    const direccion = document.getElementById('direccion').value.trim();
    const telefono = document.getElementById('telefono').value.trim();
    const email = document.getElementById('email').value.trim();

    // Validar que todos los campos estén llenos antes de actualizar
    if (!nombre || !apellido || !direccion || !telefono || !email) {
        $('#registerResult').html('<div class="alert alert-danger">Por favor, completa todos los campos antes de actualizar.</div>');
        return;  // Detener el envío si hay errores
    }

    const dueño = {
        dueñoID: editingDueñoId,  // Asegúrate de que el ID esté incluido
        nombre: nombre,
        apellido: apellido,
        direccion: direccion,
        telefono: telefono,
        email: email
    };

    fetch(`${apiUrl}/${editingDueñoId}`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(dueño)
    }).then(response => {
        if (response.ok) {
            resetForm();
            fetchDueños();
            $('#registerResult').html('<div class="alert alert-success">Dueño actualizado con éxito.</div>');
        } else {
            $('#registerResult').html('<div class="alert alert-danger">Error al actualizar el dueño.</div>');
        }
    });
});

// Función para eliminar dueño
function deleteDueño(id) {
    if (confirm('¿Estás seguro de que deseas eliminar este dueño?')) {
        fetch(`${apiUrl}/${id}`, {
            method: 'DELETE'
        }).then(response => {
            if (response.ok) {
                fetchDueños();
                $('#registerResult').html('<div class="alert alert-success">Dueño eliminado con éxito.</div>');
            } else {
                $('#registerResult').html('<div class="alert alert-danger">Error al eliminar el dueño.</div>');
            }
        });
    }
}

// Función para resetear el formulario y restaurar botones
function resetForm() {
    document.getElementById('dueñoForm').reset();
    editingDueñoId = null;

    // Restablecer los botones
    document.getElementById('registerButton').classList.remove('d-none');  // Mostrar botón de registro
    document.getElementById('updateButton').classList.add('d-none');  // Ocultar botón de actualizar
    document.getElementById('cancelButton').classList.add('d-none');  // Ocultar botón de cancelar
}

// Función para cancelar la edición
document.getElementById('cancelButton').addEventListener('click', function() {
    resetForm();
});
