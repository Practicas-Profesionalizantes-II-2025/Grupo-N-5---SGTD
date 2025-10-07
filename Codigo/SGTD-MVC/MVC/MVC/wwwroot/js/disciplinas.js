// --- UTILIDADES DISCIPLINAS ---
function ensureCancelButtonDisciplina(form) {
    let $cancel = form.find('.btn-cancel-disciplina');
    if ($cancel.length === 0) {
        $cancel = $('<button type="button" class="btn btn-eliminar btn-cancel-disciplina ms-2">Cancelar</button>');
        form.find('button[type="submit"]').after($cancel);
        $cancel.on('click', function () {
            resetFormToCreateModeDisciplina(form);
        });
    }
    $cancel.show();
}

function resetFormToCreateModeDisciplina(form) {
    form[0].reset();
    form.find('input[name="Id"]').val('');
    form.find('button[type="submit"]').text('Agregar');
    const $cancel = form.find('.btn-cancel-disciplina');
    if ($cancel.length) $cancel.hide();
}

// Modal Disciplinas
$('#modalDisciplinas').on('shown.bs.modal', function () {
    $.get(urlsDisciplina.getDisciplinas, function (data) {
        $('#tablaDisciplinasContainer').html(data);
    });
});

// Click en modificar
$(document).on('click', '.btn-modificar-disciplina', function () {
    const row = $(this).closest('tr');
    const id = row.data('id');
    const nombre = row.find('.nombre-disciplina').text().trim();

    const form = $('#formCrearDisciplina');
    form.find('input[name="Id"]').val(id);
    form.find('input[name="Nombre"]').val(nombre);
    form.find('button[type="submit"]').text('Guardar');

    ensureCancelButtonDisciplina(form);
});

// Enviar formulario
$(document).on('submit', '#formCrearDisciplina', function (e) {
    e.preventDefault();
    const form = $(this);
    const id = form.find('input[name="Id"]').val();
    const url = id ? urlsDisciplina.modificarDisciplina : urlsDisciplina.crearDisciplina;

    $.ajax({
        url: url,
        type: 'POST',
        data: form.serialize(),
        success: function (data) {
            $('#tablaDisciplinasContainer').html(data);
            form[0].reset();
            form.find('button[type="submit"]').text('Agregar');
        },
        error: function (xhr) {
            alert("Error: " + xhr.responseText);
        }
    });
});

// Eliminar Disciplina
$(document).on('click', '#modalDisciplinas .btn-eliminar-disciplina', function () {
    if (!confirm('¿Eliminar esta disciplina?')) return;

    const row = $(this).closest('tr');
    const id = row.data('id');

    $.ajax({
        url: urlsDisciplina.eliminarDisciplina + '/' + id,
        type: 'POST',
        success: function (data) {
            $('#tablaDisciplinasContainer').html(data);
        },
        error: function (xhr) {
            alert("Error al eliminar: " + xhr.responseText);
        }
    });
});
