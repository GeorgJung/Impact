# Create your views here.
from django.shortcuts import render_to_response, redirect, render
from django.template import RequestContext

def test(request):
    return render_to_response("test.html", RequestContext(request))

def home(request):
	if request.user.is_authenticated():
		return render_to_response("home.html", RequestContext(request))
	else:
		return render_to_response("register_login.html", RequestContext(request))

def login(request):
	state = ""
	if request.user.is_authenticated():
		return render_to_response("home.html", RequestContext(request))
	else:
		username = request.POST['username']
		try:
			username = User.objects.get(username=username)
		except:
			return render_to_response("register_login.html", RequestContext(request))
		password = request.POST['password']
		user = authenticate(username=username, password=password)
		if user is not None:
			login(request, user)
			state = "successfully logged in"
			return HttpResponseRedirect('/')
		else:
			state = "username/password incorrect"
			return render_to_response("register_login.html", {'state':state}, RequestContext(request))