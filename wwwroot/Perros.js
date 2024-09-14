const apiUrl = 'http://localhost:5298/api/Perros'; // Reemplaza con la URL correcta de tu API
let editingPerroId = null; // Para saber si estamos editando un perro

// Obtener todos los perros al cargar la página
window.onload = function() {
    fetchPerros();
    fetchDueños();
};


// Función para obtener todos los perros
function fetchPerros(pageNumber = 1, pageSize = 10) {
    fetch(`${apiUrl}?pageNumber=${pageNumber}&pageSize=${pageSize}`)
        .then(response => response.json())
        .then(data => {
            const perrosList = document.getElementById('perrosList');
            perrosList.innerHTML = '';
            data.forEach(perro => {
                const edad = calcularEdad(perro.fechaNacimiento);
                perrosList.innerHTML += `
                    <tr>
                        <td>${perro.perroID}</td>
                        <td>${perro.nombre}</td>
                        <td>${perro.raza}</td>
                        <td>${new Date(perro.fechaNacimiento).toLocaleDateString()}</td>
                        <td>${edad} años</td>
                        <td>${perro.peso}</td>
                        <td>${perro.tamaño}</td>
                        <td>${perro.sexo}</td>
                        <td>${perro.dueñoNombre}</td>
                      <td>
                            <button class="btn btn-info" onclick="consultarPerro(${perro.perroID})">
                                <i class="bi bi-eye"></i> Consultar
                            </button>
                            <button class="btn btn-warning" onclick="editPerro(${perro.perroID})">
                                <i class="bi bi-pencil-square"></i> Editar
                            </button>
                            <button class="btn btn-danger" onclick="deletePerro(${perro.perroID})">
                                <i class="bi bi-trash-fill"></i> Eliminar
                            </button>
                        </td>
                    </tr>
                `;
            });
        });
}

// Función para calcular la edad a partir de la fecha de nacimiento
function calcularEdad(fechaNacimiento) {
    const hoy = new Date();
    const nacimiento = new Date(fechaNacimiento);
    let edad = hoy.getFullYear() - nacimiento.getFullYear();
    const mes = hoy.getMonth() - nacimiento.getMonth();
    if (mes < 0 || (mes === 0 && hoy.getDate() < nacimiento.getDate())) {
        edad--;
    }
    return edad;
}


// Función para obtener todos los dueños
function fetchDueños() {
    fetch('http://localhost:5298/api/Dueños')
        .then(response => response.json())
        .then(data => {
            const dueñoSelect = document.getElementById('dueñoID');
            data.forEach(dueño => {
                const option = document.createElement('option');
                option.value = dueño.dueñoID;
                option.text = `${dueño.nombre} ${dueño.apellido}`;
                dueñoSelect.add(option);
            });
        });
}

// ** Compresión de imagen al seleccionar archivo **
document.getElementById('foto').addEventListener('change', async (event) => {
    const file = event.target.files[0];

    if (file) {
        try {
            const options = {
                maxSizeMB: 2,
                maxWidthOrHeight: 800,
                useWebWorker: true
            };
            const compressedFile = await imageCompression(file, options);
            const formData = new FormData();
            formData.append('foto', compressedFile);

            console.log('Archivo comprimido:', compressedFile);
        } catch (error) {
            console.error('Error durante la compresión:', error);
        }
    }
});


// Registrar nuevo perro
// Registrar nuevo perro
document.getElementById('registerButton').addEventListener('click', function(event) {
    event.preventDefault();

    const fotoInput = document.getElementById('foto');
    const formData = new FormData();
    
    formData.append('nombre', document.getElementById('nombre').value);
    formData.append('raza', document.getElementById('raza').value);
    formData.append('fechaNacimiento', document.getElementById('fechaNacimiento').value);
    formData.append('sexo', document.getElementById('sexo').value); // Añadir el sexo
    formData.append('peso', document.getElementById('peso').value);
    formData.append('tamaño', document.getElementById('tamaño').value);
    formData.append('dueñoID', document.getElementById('dueñoID').value);

    // Si hay una imagen seleccionada, compimir antes de enviarla
    if (fotoInput.files.length > 0) {
        const file = fotoInput.files[0];

        // Comprimir la imagen
        const options = {
            maxSizeMB: 2,
            maxWidthOrHeight: 800,
            useWebWorker: true
        };

        imageCompression(file, options)
            .then(compressedFile => {
                formData.append('foto', compressedFile);
                return fetch(apiUrl, {
                    method: 'POST',
                    body: formData
                });
            })
            .then(response => {
                if (response.ok) {
                    resetForm();
                    fetchPerros();
                    document.getElementById('registerResult').innerHTML = '<div class="alert alert-success">Perro registrado con éxito.</div>';
                } else {
                    document.getElementById('registerResult').innerHTML = '<div class="alert alert-danger">Error al registrar el perro.</div>';
                }
            })
            .catch(error => console.error('Error durante el registro:', error));
    } else {
        // Si no hay imagen seleccionada, envía el formulario sin compresión
        fetch(apiUrl, {
            method: 'POST',
            body: formData
        }).then(response => {
            if (response.ok) {
                resetForm();
                fetchPerros();
                document.getElementById('registerResult').innerHTML = '<div class="alert alert-success">Perro registrado con éxito.</div>';
            } else {
                document.getElementById('registerResult').innerHTML = '<div class="alert alert-danger">Error al registrar el perro.</div>';
            }
        });
    }
});

// Función para cargar los datos del perro en el formulario para editar
function editPerro(id) {
    fetch(`${apiUrl}/${id}`)
        .then(response => response.json())
        .then(perro => {
            // Asignar los valores del perro a los campos del formulario
            document.getElementById('nombre').value = perro.nombre;
            document.getElementById('raza').value = perro.raza;
            document.getElementById('fechaNacimiento').value = new Date(perro.fechaNacimiento).toISOString().split('T')[0];  // Fecha de nacimiento
            document.getElementById('peso').value = perro.peso;
            document.getElementById('tamaño').value = perro.tamaño;
            document.getElementById('sexo').value = perro.sexo;  // Sexo
            document.getElementById('dueñoID').value = perro.dueñoID;
            editingPerroId = perro.perroID;

            // Cambiar botones
            document.getElementById('registerButton').style.display = 'none';  // Ocultar el botón de registro
            document.getElementById('updateButton').style.display = 'inline-block';  // Mostrar el botón de actualizar
            document.getElementById('cancelButton').style.display = 'inline-block';  // Mostrar el botón de cancelar
        })
        .catch(error => {
            console.error('Error al cargar los datos del perro:', error);
        });
}

// Función para consultar los datos de un perro sin poder editarlos
function consultarPerro(id) {
    fetch(`${apiUrl}/${id}`)
        .then(response => response.json())
        .then(perro => {
            // Asignar los valores del perro a los campos del formulario
            document.getElementById('nombre').value = perro.nombre;
            document.getElementById('raza').value = perro.raza;
            document.getElementById('fechaNacimiento').value = new Date(perro.fechaNacimiento).toISOString().split('T')[0];  // Fecha de nacimiento
            document.getElementById('peso').value = perro.peso;
            document.getElementById('tamaño').value = perro.tamaño;
            document.getElementById('sexo').value = perro.sexo;
            document.getElementById('dueñoID').value = perro.dueñoID;

            // Mostrar la imagen del perro si existe
            if (perro.foto) {
                const imageUrl = `data:image/jpeg;base64,${perro.foto}`;  // Asignar la cadena base64 como imagen
                document.getElementById('fotoPreview').src = imageUrl;
                document.getElementById('fotoPreview').style.display = 'block';
            } else {
                document.getElementById('fotoPreview').style.display = 'none';  // Ocultar si no hay foto
            }

            // Deshabilitar los campos para que no puedan editarse
            document.getElementById('nombre').disabled = true;
            document.getElementById('raza').disabled = true;
            document.getElementById('fechaNacimiento').disabled = true;
            document.getElementById('peso').disabled = true;
            document.getElementById('tamaño').disabled = true;
            document.getElementById('sexo').disabled = true;
            document.getElementById('dueñoID').disabled = true;
            document.getElementById('foto').disabled = true;

            // Ocultar los botones de edición y mostrar solo el botón de cancelar
            document.getElementById('registerButton').style.display = 'none';
            document.getElementById('updateButton').style.display = 'none';
            document.getElementById('cancelButton').style.display = 'inline-block';  // Solo el botón de cancelar
        })
        .catch(error => {
            console.error('Error al cargar los datos del perro:', error);
        });
}




// Actualizar perro
document.getElementById('updateButton').addEventListener('click', async function(event) {
    event.preventDefault();

    const formData = new FormData();
    formData.append('PerroID', editingPerroId); // Asegúrate de enviar el ID
    formData.append('Nombre', document.getElementById('nombre').value);
    formData.append('Raza', document.getElementById('raza').value);
    formData.append('FechaNacimiento', document.getElementById('fechaNacimiento').value);  // Fecha de Nacimiento
    formData.append('Sexo', document.getElementById('sexo').value);  // Sexo
    formData.append('Peso', document.getElementById('peso').value);
    formData.append('Tamaño', document.getElementById('tamaño').value);
    formData.append('DueñoID', document.getElementById('dueñoID').value);

    const fotoInput = document.getElementById('foto');
    if (fotoInput.files.length > 0) {
        const file = fotoInput.files[0];
        const options = {
            maxSizeMB: 2,
            maxWidthOrHeight: 800,
            useWebWorker: true
        };
        try {
            const compressedFile = await imageCompression(file, options);
            formData.append('Foto', compressedFile); // Asegúrate de usar 'Foto'
        } catch (error) {
            console.error('Error al comprimir la imagen:', error);
        }
    }

    // Mostrar todo lo que contiene el FormData (para depurar)
    for (let pair of formData.entries()) {
        console.log(pair[0] + ': ' + pair[1]);
    }

    fetch(`${apiUrl}/${editingPerroId}`, {
        method: 'PUT',
        body: formData
    }).then(response => {
        if (response.ok) {
            resetForm();
            fetchPerros();
            document.getElementById('registerResult').innerHTML = '<div class="alert alert-success">Perro actualizado con éxito.</div>';
        } else {
            document.getElementById('registerResult').innerHTML = '<div class="alert alert-danger">Error al actualizar el perro.</div>';
        }
    }).catch(error => {
        console.error('Error durante la actualización:', error);
    });
});



// Función para eliminar perro
function deletePerro(id) {
    if (confirm('¿Estás seguro de que deseas eliminar este perro?')) {
        fetch(`${apiUrl}/${id}`, {
            method: 'DELETE'
        }).then(response => {
            if (response.ok) {
                // Eliminar la fila directamente del DOM para evitar volver a cargar toda la lista
                const rowToDelete = document.querySelector(`#perrosList tr[data-id='${id}']`);
                if (rowToDelete) {
                    rowToDelete.remove();
                }
                document.getElementById('registerResult').innerHTML = '<div class="alert alert-success">Perro eliminado con éxito.</div>';
            } else {
                document.getElementById('registerResult').innerHTML = '<div class="alert alert-danger">Error al eliminar el perro.</div>';
            }
        }).catch(error => {
            console.error('Error durante la eliminación:', error);
        });
    }
}
// Filtrar mascotas por nombre
document.getElementById('searchInput').addEventListener('input', function () {
    const searchValue = this.value.toLowerCase();
    const perrosList = document.getElementById('perrosList').querySelectorAll('tr');
    
    perrosList.forEach(function (row) {
        const nombre = row.querySelector('td:nth-child(2)').textContent.toLowerCase();
        if (nombre.includes(searchValue)) {
            row.style.display = ''; // Mostrar la fila si coincide
        } else {
            row.style.display = 'none'; // Ocultar si no coincide
        }
    });
});



// Función para resetear el formulario
function resetForm() {
    document.getElementById('perroForm').reset();
    editingPerroId = null;
    // Habilitar los campos
    document.getElementById('nombre').disabled = false;
    document.getElementById('raza').disabled = false;
    document.getElementById('fechaNacimiento').disabled = false;
    document.getElementById('peso').disabled = false;
    document.getElementById('tamaño').disabled = false;
    document.getElementById('sexo').disabled = false;
    document.getElementById('dueñoID').disabled = false;
    document.getElementById('foto').disabled = false;

    // Ocultar la previsualización de la imagen
    document.getElementById('fotoPreview').style.display = 'none';

    // Restablecer los botones
    document.getElementById('registerButton').style.display = 'inline-block';
    document.getElementById('updateButton').style.display = 'none';
    document.getElementById('cancelButton').style.display = 'none';
}

// Función para cancelar la edición
document.getElementById('cancelButton').addEventListener('click', function() {
    resetForm();
});
