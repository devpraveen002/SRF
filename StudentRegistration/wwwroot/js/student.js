$(document).ready(function () {
    // Load student list
    loadStudentList();

    // Save button click event
    $("#saveBtn").click(function () {
        if (validateForm()) {
            saveStudent();
        }
    });

    // Clear button click event
    $("#clearBtn").click(function () {
        clearForm();
    });

    // Form validation function
    function validateForm() {
        let isValid = true;

        // Reset error messages
        $(".text-danger").text("");

        // Name validation (Alpha Numeric only)
        const name = $("#name").val().trim();
        if (name === "") {
            $("#nameError").text("Name is required");
            isValid = false;
        } else if (!/^[a-zA-Z0-9\s]+$/.test(name)) {
            $("#nameError").text("Name should contain only alphanumeric characters");
            isValid = false;
        }

        // Mobile validation (10 digits, numeric only)
        const mobile = $("#mobile").val().trim();
        if (mobile === "") {
            $("#mobileError").text("Mobile number is required");
            isValid = false;
        } else if (!/^[0-9]{10}$/.test(mobile)) {
            $("#mobileError").text("Mobile number should be 10 digits");
            isValid = false;
        }

        // Gender validation
        const gender = $("input[name='gender']:checked").val();
        if (!gender) {
            $("#genderError").text("Gender is required");
            isValid = false;
        }

        // Email validation
        const email = $("#email").val().trim();
        if (email === "") {
            $("#emailError").text("Email is required");
            isValid = false;
        } else if (!/^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/.test(email)) {
            $("#emailError").text("Invalid email format");
            isValid = false;
        }

        // DOB validation
        const dob = $("#dob").val();
        if (dob === "") {
            $("#dobError").text("Date of Birth is required");
            isValid = false;
        }

        // Class validation (Alpha Numeric only)
        const className = $("#class").val().trim();
        if (className === "") {
            $("#classError").text("Class is required");
            isValid = false;
        } else if (!/^[a-zA-Z0-9\s]+$/.test(className)) {
            $("#classError").text("Class should contain only alphanumeric characters");
            isValid = false;
        }

        // Father's name validation (Alpha only)
        const fatherName = $("#fatherName").val().trim();
        if (fatherName === "") {
            $("#fatherNameError").text("Father's name is required");
            isValid = false;
        } else if (!/^[a-zA-Z\s]+$/.test(fatherName)) {
            $("#fatherNameError").text("Father's name should contain only alphabets");
            isValid = false;
        }

        // Mother's name validation (Alpha only)
        const motherName = $("#motherName").val().trim();
        if (motherName === "") {
            $("#motherNameError").text("Mother's name is required");
            isValid = false;
        } else if (!/^[a-zA-Z\s]+$/.test(motherName)) {
            $("#motherNameError").text("Mother's name should contain only alphabets");
            isValid = false;
        }

        return isValid;
    }

    // Save student function using AJAX
    function saveStudent() {
        const studentId = $("#studentId").val();
        const studentData = {
            Id: studentId,
            Name: $("#name").val().trim(),
            Mobile: $("#mobile").val().trim(),
            Gender: $("input[name='gender']:checked").val(),
            Email: $("#email").val().trim(),
            Address: $("#address").val().trim(),
            DOB: $("#dob").val(),
            Class: $("#class").val().trim(),
            FatherName: $("#fatherName").val().trim(),
            MotherName: $("#motherName").val().trim()
        };

        $.ajax({
            url: studentId == 0 ? '/Student/Create' : '/Student/Edit/' + studentId,
            type: 'POST',
            data: studentData,
            headers: {
                'RequestVerificationToken': $('input:hidden[name="__RequestVerificationToken"]').val()
            },
            success: function (result) {
                if (result.success) {
                    $("#successMessage").text(result.message || "Student saved successfully!").show();
                    $("#errorMessage").hide();
                    clearForm();
                    loadStudentList();

                    // Hide success message after 3 seconds
                    setTimeout(function () {
                        $("#successMessage").hide();
                    }, 3000);
                } else {
                    $("#errorMessage").text(result.message || "Error saving student").show();
                    $("#successMessage").hide();

                    // Show validation errors if any
                    if (result.errors && result.errors.length > 0) {
                        $.each(result.errors, function (index, error) {
                            $("#errorMessage").append("<br>" + error);
                        });
                    }
                }
            },
            error: function (xhr, status, error) {
                $("#errorMessage").text("An error occurred: " + error).show();
                $("#successMessage").hide();
            }
        });
    }

    // Load student list function
    function loadStudentList() {
        $.ajax({
            url: '/Student/GetAll',
            type: 'GET',
            success: function (data) {
                const tableBody = $("#studentTable tbody");
                tableBody.empty();

                $.each(data, function (index, student) {
                    const row = `
                        <tr>
                            <td>${student.name}</td>
                            <td>${student.mobile}</td>
                            <td>${student.email}</td>
                            <td>${student.class}</td>
                            <td>
                                <button class="btn btn-sm btn-warning edit-btn" data-id="${student.id}">Edit</button>
                                <button class="btn btn-sm btn-danger delete-btn" data-id="${student.id}">Delete</button>
                            </td>
                        </tr>
                    `;
                    tableBody.append(row);
                });

                // Add event handlers for edit and delete buttons
                $(".edit-btn").click(function () {
                    const studentId = $(this).data("id");
                    loadStudentForEdit(studentId);
                });

                $(".delete-btn").click(function () {
                    const studentId = $(this).data("id");
                    if (confirm("Are you sure you want to delete this student?")) {
                        deleteStudent(studentId);
                    }
                });
            },
            error: function (xhr, status, error) {
                $("#errorMessage").text("Error loading student list: " + error).show();
            }
        });
    }

    // Load student for edit function
    function loadStudentForEdit(id) {
        $.ajax({
            url: '/Student/Edit/' + id,
            type: 'GET',
            success: function (response) {
                // Extract student data from the HTML response
                const parser = new DOMParser();
                const htmlDoc = parser.parseFromString(response, 'text/html');

                $("#studentId").val(id);
                $("#name").val($(htmlDoc).find('#Name').val());
                $("#mobile").val($(htmlDoc).find('#Mobile').val());
                $(`input[name='gender'][value='${$(htmlDoc).find('#Gender').val()}']`).prop('checked', true);
                $("#email").val($(htmlDoc).find('#Email').val());
                $("#address").val($(htmlDoc).find('#Address').val());
                $("#dob").val($(htmlDoc).find('#DOB').val().split('T')[0]);
                $("#class").val($(htmlDoc).find('#Class').val());
                $("#fatherName").val($(htmlDoc).find('#FatherName').val());
                $("#motherName").val($(htmlDoc).find('#MotherName').val());

                $("#saveBtn").text("Update");
                $('html, body').animate({
                    scrollTop: $("#studentForm").offset().top
                }, 500);
            },
            error: function (xhr, status, error) {
                $("#errorMessage").text("Error loading student: " + error).show();
            }
        });
    }

    // Delete student function
    function deleteStudent(id) {
        $.ajax({
            url: '/Student/Delete',
            type: 'POST',
            data: { id: id },
            headers: {
                'RequestVerificationToken': $('input:hidden[name="__RequestVerificationToken"]').val()
            },
            success: function (result) {
                if (result.success) {
                    $("#successMessage").text("Student deleted successfully!").show();
                    $("#errorMessage").hide();
                    loadStudentList();

                    // Hide success message after 3 seconds
                    setTimeout(function () {
                        $("#successMessage").hide();
                    }, 3000);
                } else {
                    $("#errorMessage").text("Error deleting student").show();
                    $("#successMessage").hide();
                }
            },
            error: function (xhr, status, error) {
                $("#errorMessage").text("An error occurred: " + error).show();
                $("#successMessage").hide();
            }
        });
    }

    // Clear form function
    function clearForm() {
        $("#studentId").val("0");
        $("#studentForm")[0].reset();
        $(".text-danger").text("");
        $("#saveBtn").text("Save");
        $("#successMessage, #errorMessage").hide();
    }
});