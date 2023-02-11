document.addEventListener('DOMContentLoaded', async () => {

  const stripe = Stripe('pk_test_51MPWcXKl1bbAP6TZKGlreCDo1ldI4VsETmeKAYI4g6ItCydYFUtHYyBQMRIRaOL2k1rqX6qL0OiXWRfwGeXFRRMI00TZKe1ZbR');

  const elements = stripe.elements();

  const cardNumber = elements.create('cardNumber');
  cardNumber.mount('#cardNumber');

  const cardExpiry = elements.create('cardExpiry');
  cardExpiry.mount('#cardExpiry');

  const cardCvc = elements.create('cardCvc');
  cardCvc.mount('#cardCvc');

  const form = document.getElementById('payment-form');
  
  form.addEventListener('submit', async (e) => {
    e.preventDefault();

    // Creating payment intent at the same time of confirming the payment for testing purposes
    const { paymentIntentId, clientSecret } = await fetch(
      'http://localhost:9999/api/v1/enrolling/enrollment/payment-intent',
      {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6ImNjNTlhNDRmLWM1NDctNGYwYi1iMmQ1LTcyMDBiODJmNTNiYiIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlVzZXIiLCJleHAiOjE2NzY2MjAyNjcsImlzcyI6Im51bGwiLCJhdWQiOiJudWxsIn0.B6yCL-5xrET1xOGB3gfTOBtnjPYPBunBKJV6IXRhscE'
        },
        body: JSON.stringify({
          "courseId": 5,
          "price": 59
        }),
      }
    ).then((result) => result.json());

    if (!clientSecret) {
      console.log("Could not get response from server");
      return;
    }

    console.log(`Payment intent created successfully.`);

    const nameInput = document.querySelector('#nameOnCard');

    const {error: stripeError, paymentIntent} = await stripe.confirmCardPayment(
      clientSecret, {
        payment_method: {
          card: cardNumber,
          billing_details: {
            name: nameInput.value,
          },
        },
      }
    );

    if (stripeError) {
      console.log(stripeError.message);
    } else {
      var notesElement = document.querySelector('.notes');
      var paymentStatusContentElement = document.querySelector('.payment-status span:last-child');
      notesElement.style.display = 'block';
      paymentStatusContentElement.textContent = paymentIntent.status;
    }
  });
});
