if (localStorage.getItem("email")) {
  document.getElementById("email").value = localStorage.getItem("email");
}

if (document.cookie.includes("token")) {
  // if isAdmin is true
  if (document.cookie.includes("isAdmin=true")) {
    window.location.href = "/AdminDashboard";
  } else {
    window.location.href = "/Dashboard";
  }
}

function showAndHidePassword(flag) {
  const password = document.getElementById("password");
  const showPasswordIcon = document.getElementById("show-password");
  const hidePasswordIcon = document.getElementById("hide-password");
  if (flag) {
    password.type = "text";
    showPasswordIcon.classList.add("d-none");
    hidePasswordIcon.classList.remove("d-none");
  } else {
    password.type = "password";
    showPasswordIcon.classList.remove("d-none");
    hidePasswordIcon.classList.add("d-none");
  }
}

$("#email").on("input", function () {
  localStorage.setItem("email", $("#email").val());
});

$(document).ready(function () {
  $("#login-form").submit(function (e) {
    e.preventDefault();

    if (!$(this).valid()) {
      return;
    }

    var email = $("#email").val();
    var password = $("#password").val();
    var rememberMe = $("#remember-me").is(":checked");

    $.ajax({
      url: "/api/validate",
      type: "POST",
      contentType: "application/json",
      data: JSON.stringify({
        email: email,
        password: password,
      }),
      success: function (response) {
        if (response.success) {
          if (rememberMe) {
            document.cookie = `isAdmin=${response.isAdmin}; max-age=${60 * 60 * 24 * 7}`;
            document.cookie = `token=${response.token}; max-age=${60 * 60 * 24 * 7}`;
          } else {
            document.cookie = `isAdmin=${response.isAdmin};`;
            document.cookie = `token=${response.token};`;
          }
          toastr.success(response.message);
          if (response.isAdmin) {
            setTimeout(() => {
              window.location.href = "/AdminDashboard";
            }, 1000);
          } else {
            setTimeout(() => {
              window.location.href = "/Dashboard";
            }, 1000);
          }
        } else {
          toastr.error(response.message);
        }
      },
      error: function () {
        toastr.error("An error occurred while processing your request.");
      },
    });
  });
});
