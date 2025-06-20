function parseJwt(token) {
    const payload = token.split('.')[1];
    return JSON.parse(atob(payload));
}

let id, nombre, token;

function cargarUsuario() {
    token = sessionStorage.getItem("token");

    if (!token) {
        window.location.href = "index.html";
        return;
    }

    const claims = parseJwt(token);
    id = claims.id;
    nombre = claims["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"];

    // Solo si el elemento está presente en la vista
    const nombreDisplay = document.getElementById("nombreUser");
    if (nombreDisplay) {
        nombreDisplay.textContent = `Bienvenido, ${nombre}`;
    }

    const botonSalir = document.querySelector("#exit");
    if (botonSalir) {
        botonSalir.addEventListener("click", cerrarSesion);
    }
}

function cerrarSesion() {
    sessionStorage.removeItem("token");
    window.location.href = "index.html";
}

cargarUsuario();

// Inicializar conexión SignalR (solo si existe en la vista)
if (window.signalR) {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:44341/hubs/encuesta", {
            accessTokenFactory: () => sessionStorage.getItem("token")
        })
        .withAutomaticReconnect()
        .build();

    connection.on("updateCount", function (count) {
        const el = document.getElementById("entrevistadoresActivos");
        if (el) {
            el.innerHTML = `${count}<br><span>Entrevistadores Activos</span>`;
        }
    });



    //connection.on("RespuestasRecibidas", (dto) => {
    //    console.log("Datos recibidos en tiempo real:", dto);

    //    const tarjetas = {
    //        encuestasCreadas: dto.cantidadEncuestas,
    //        respuestasRecibidas: dto.respuestasRecibidas,
    //        estudiantesEncuestados: dto.cantidadDeAlumnos
    //    };

    //    for (let id in tarjetas) {
    //        const el = document.getElementById(id);
    //        if (el) {
    //            el.innerHTML = `${tarjetas[id]}<br><span>${el.querySelector("span")?.textContent || ""}</span>`;
    //        }
    //    }
    //    console.log("actualizado");
    //    // Actualizar tabla de encuestas
    //    const cuerpoTabla = document.getElementById("tablaEncuestas");
    //    if (cuerpoTabla) {
    //        cuerpoTabla.innerHTML = "";

    //        if (dto.lstEncuestasDisponibles.length > 0) {
    //            dto.lstEncuestasDisponibles.forEach(encuesta => {
    //                const fila = document.createElement("tr");
    //                const preguntasTexto = encuesta.lstPreguntaEncuesta?.map(p => p.texto).join(", ") || "Sin preguntas";
    //                const fecha = new Date(encuesta.fechaCreacion).toLocaleDateString();
    //                const creador = encuesta.lstUsuarioCreador?.map(u => u.nombre).join(", ") || "Desconocido";

    //                fila.innerHTML = `
    //                    <td>${encuesta.titulo}</td>
    //                    <td>${creador}</td>
    //                    <td>${fecha}</td>
    //                    <td><button class="btnEliminar" data-id="${encuesta.id}">🗑️ Eliminar</button></td>`;
    //                cuerpoTabla.appendChild(fila);
    //            });
    //        } else {
    //            const filaVacia = document.createElement("tr");
    //            filaVacia.innerHTML = `<td colspan="6" class="empty">No hay encuestas disponibles</td>`;
    //            cuerpoTabla.appendChild(filaVacia);
    //        }
    //    }
    //});

    function actualizarVistaConDatos(dto) {
        const tarjetas = {
            encuestasCreadas: dto.cantidadEncuestas,
            respuestasRecibidas: dto.respuestasRecibidas,
            estudiantesEncuestados: dto.cantidadDeAlumnos
        };

        for (let id in tarjetas) {
            const el = document.getElementById(id);
            if (el) {
                el.innerHTML = `${tarjetas[id]}<br><span>${el.querySelector("span")?.textContent || ""}</span>`;
            }
        }

        const cuerpoTabla = document.getElementById("tablaEncuestas");
        if (!cuerpoTabla) return;

        cuerpoTabla.innerHTML = "";

        if (dto.lstEncuestasDisponibles?.length > 0) {
            dto.lstEncuestasDisponibles.forEach(encuesta => {
                const fila = document.createElement("tr");

                const preguntasTexto = encuesta.lstPreguntaEncuesta?.map(p => p.texto).join(", ") || "Sin preguntas";
                const fecha = new Date(encuesta.fechaCreacion);
                const creador = encuesta.lstUsuarioCreador?.map(u => u.nombre).join(", ") || "Desconocido";

                fila.innerHTML = `
                <td data-label="Título">${encuesta.titulo}</td>
                <td data-label="Creado por">${creador}</td>
                <td data-label="Fecha">${fecha.toLocaleDateString()}</td>
                <td>
                    <button class="btnEliminar" data-id="${encuesta.id}">🗑️ Eliminar</button>
                </td>
            `;

                // Asignar evento al botón de eliminar
                fila.querySelector(".btnEliminar").addEventListener("click", async () => {
                    if (!confirm("¿Estás seguro de eliminar esta encuesta?")) return;

                    try {
                        const res = await fetch(`https://localhost:44341/api/encuesta/${encuesta.id}`, {
                            method: "DELETE",
                            headers: {
                                "Authorization": "Bearer " + sessionStorage.getItem("token")
                            }
                        });

                        const msg = await res.text();
                        if (!res.ok) throw new Error(msg);

                        alert("✅ " + msg);
                        // Puedes volver a pedir los datos actualizados o usar SignalR para ello
                    } catch (err) {
                        alert("❌ Error: " + err.message);
                    }
                });

                cuerpoTabla.appendChild(fila);
            });
        } else {
            const filaVacia = document.createElement("tr");
            filaVacia.innerHTML = `<td colspan="6" class="empty">No hay encuestas disponibles</td>`;
            cuerpoTabla.appendChild(filaVacia);
        }
    }
    connection.on("RespuestasRecibidas", actualizarVistaConDatos);



    connection.start()
        .then(() => console.log("Conectado al Hub de encuestas"))
        .catch(err => console.error("Error al conectar con el Hub:", err));
}
