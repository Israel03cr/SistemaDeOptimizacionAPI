document.getElementById('loginForm').addEventListener('submit', function(event) {
    event.preventDefault();

    const data = {
        Email: document.getElementById('email').value,
        Password: document.getElementById('password').value
    };

    fetch('http://localhost:5298/api/Account/login', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    })
    .then(response => response.json())
    .then(response => {
        if (response.token) {
            // Guarda el token en el localStorage
            localStorage.setItem('token', response.token);

            // Muestra mensaje de éxito
            document.getElementById('loginResult').innerHTML = '<div class="alert alert-success">Inicio de sesión exitoso</div>';

            // Redirigir al menú principal (secundario)
            setTimeout(function() {
                window.location.href = 'Menu.html'; // Cambia a la URL del menú principal
            }, 2000); // Espera 2 segundos antes de redirigir
        } else {
            document.getElementById('loginResult').innerHTML = '<div class="alert alert-danger">Error en la autenticación</div>';
        }
    })
    .catch(error => {
        document.getElementById('loginResult').innerHTML = '<div class="alert alert-danger">Error: ' + error + '</div>';
    });
});
