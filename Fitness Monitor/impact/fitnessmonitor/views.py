# Create your views here.
from django.shortcuts import render_to_response, redirect, render
from django.template import RequestContext
from fitnessmonitor.models import Practitioner
from django.http import HttpResponseRedirect, HttpResponse
from django.contrib.auth import logout
from django.contrib.auth import authenticate, login

def test(request):
    return render_to_response("test.html", RequestContext(request))

def home(request):
	if request.user.is_authenticated():
		return render_to_response("home.html", RequestContext(request))
	else:
		return render_to_response("register_login.html", RequestContext(request))

def login_user(request):
	state = ""
	if request.user.is_authenticated():
		return HttpResponseRedirect('/')
	else:
		username = request.POST['username_login']
		try:
			username = Practitioner.objects.get(username=username)
		except:
			state = "please register"
			return render_to_response("register_login.html", {'state':state}, RequestContext(request))
		password = request.POST['password_login']
		user = authenticate(username=username, password=password)
		if user is not None:
			login(request, user)
			state = "successfully logged in"
			return HttpResponseRedirect('/')
		else:
			state = "username/password incorrect"
			return render_to_response("register_login.html", {'state':state}, RequestContext(request))

def register(request):
	if request.user.is_authenticated():
		return render_to_response("home.html", RequestContext(request))

	username = request.POST['username_register']
	password = request.POST['password_register']
	practitioner = Practitioner.objects.create(username=username)
	practitioner.set_password(password)
	practitioner.save()
	return HttpResponseRedirect('/')

def new_session(request):
	return render_to_response("session.html", RequestContext(request))
	
def signout(request):
	logout(request)
	return HttpResponseRedirect('/')