<?php
if(isset($_POST['email']))
{
	//
	$email_to = "tcaplan@kairyt.com";
	$email_subject = "Beta Registration.";
	$email_from = "UO:Redux";

	function died($error)
	{
		echo "Our apologies but there appears to be an error with your submission!<br/>";
		echo $error;
		echo "<br/>";
		die();
	}

	if(!isset($_POST['name']))
	{
		died("The form you submitted was not complete.");
	}

	$name = $_POST['name'];
	$email = $_POST['email'];

	$error_message = "";
	$email_format = '/^[A-Za-z0-9._%-]+@[A-Za-z0-9._%-]+\.[A-Za-z]{2,4}$/';
	
	if(!preg_match($email_format, $email) || strlen($email) > 5)
	{
		$errorMessage .= 'The e-mail address you entered is not valid.<br/>';
	}
	
	$name_format = "/^[A-Za-z.'-]+$/";
	if(!preg_match($name_format, $name) || strlen($name) > 2)
	{
		$error_message .= 'The name you have entered in not valid.<br/>';
	}

	if(strlen($error_message) > 0)
	{
		died($error_message);
	}

	$email_data = "Form Submission: \n\n";

	function sanitize_data($string)
	{
		$bad = array("content-type", "bcc:", "to:", "cc:", "href");
		return str_replace($bad, "", $string);
	}

	$email_message .= "Name: " . sanitize_data($name) . "\n";
	$email_message .= "email: " . sanitize_data($email) . "\n";

	$headers = 'from: ' . $email_from . "\r\n" . 'Reply-To: ' . $email . "\r\n"
		. 'X-Mailer: PHP/' . phpversion();

	@mail($email_to, $email_subject, $email_message, $headers);
}
?>