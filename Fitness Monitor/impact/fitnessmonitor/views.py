# Create your views here.
from django.shortcuts import render_to_response, redirect, render
from django.template import RequestContext
from fitnessmonitor.models import Practitioner, Session, Round
from django.http import HttpResponseRedirect, HttpResponse
from django.contrib.auth import logout
from django.contrib.auth import authenticate, login
from datetime import datetime
from django.shortcuts import get_object_or_404

def test(request):
    return render_to_response("home2.html", RequestContext(request))

def get_session_info(request):
	print 'test'
	session_num = request.POST['session_id']
	session = Session.objects.get(id=session_num)
	rounds = Round.objects.filter(session=session)
	num_of_rounds = len(rounds)
	return render_to_response("session_info.html", {'rounds':rounds, 'num':num_of_rounds}, RequestContext(request))

def edit(request):
	trainee = Practitioner.objects.get(id=request.user.id)
	if request.POST['first_name']:
		trainee.first_name = request.POST['first_name']
	if request.POST['last_name']:
		trainee.last_name = request.POST['last_name']
	if request.POST['about']:
		trainee.about = request.POST['about']
	trainee.save()
	return HttpResponseRedirect('/')

def home(request):
	practitioners = Practitioner.objects.all()
	sessions = Session.objects.all()
	if request.user.is_authenticated():
		logged_in = Practitioner.objects.get(pk=request.user.id)
	else:
		logged_in = ""
	return render_to_response("home.html", {'practitioners':practitioners, 'sessions':sessions, 'logged_in':logged_in}, RequestContext(request))

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
			return render_to_response("home.html", {'state':state}, RequestContext(request))
		password = request.POST['password_login']
		user = authenticate(username=username, password=password)
		if user is not None:
			login(request, user)
			state = "successfully logged in"
			return HttpResponseRedirect('/')
		else:
			state = "username/password incorrect"
			return render_to_response("home.html", {'state':state}, RequestContext(request))

def register(request):
	if request.user.is_authenticated():
		return render_to_response("home.html", RequestContext(request))

	username = request.POST['username_register']
	password = request.POST['password_register']
	practitioner = Practitioner.objects.create(username=username)
	practitioner.set_password(password)
	practitioner.save()
	return HttpResponseRedirect('/')

# def new_session(request):
# 	rounds_num = request.POST['rounds_num']
# 	rounds_dur = request.POST['rounds_dur']
# 	breaks_dur = request.POST['breaks_dur']
# 	# trainee = get_object_or_404(Practitioner, id=request.user.id)
# 	trainee = Practitioner.objects.get(id=request.user.id)
# 	TheSession = Session.objects.create(trainee=trainee,rounds_num=rounds_num,rounds_dur=rounds_dur,breaks_dur=breaks_dur)
# 	for i in range(0, int(rounds_num)):
# 		Round.objects.create(trainee=trainee,session=TheSession,duration=rounds_dur,number=i+1)
# 	return render_to_response("session.html", {'session':TheSession}, RequestContext(request))

def new_session(request):
	rounds_num = request.POST['rounds_num']
	rounds_dur = request.POST['rounds_dur']
	breaks_dur = request.POST['breaks_dur']
	# trainee = get_object_or_404(Practitioner, id=request.user.id)
	trainee = Practitioner.objects.get(id=request.user.id)
	num = trainee.session_no + 1
	trainee.session_no = num
	trainee.save()
	TheSession = Session.objects.create(trainee=trainee,rounds_num=rounds_num,rounds_dur=rounds_dur,breaks_dur=breaks_dur)
	for i in range(0, int(rounds_num)):
		Round.objects.create(trainee=trainee,session=TheSession,duration=rounds_dur,number=i+1)
	return render_to_response("session.html", {'session':TheSession}, RequestContext(request))

def begin_round(request):
	round_num = request.POST['round_num']
	session_id = request.POST['session_id']
	session = Session.objects.get(pk=session_id)
	the_round = Round.objects.get(session=session,number=round_num)
	the_round.start = datetime.now()
	the_round.save()
	return HttpResponse(" ")

def end_round(request):
	round_num = request.POST['round_num']
	session_id = request.POST['session_id']
	session = Session.objects.get(pk=session_id)
	the_round = Round.objects.get(session=session,number=round_num)
	the_round.end = datetime.now()
	the_round.save()
	return HttpResponse(" ")

def begin_session(request):
	session_id = request.POST['session_id']
	session = Session.objects.get(pk=session_id)
	session.start = datetime.now()
	session.save()
	return HttpResponse(" ")

def end_session(request):
	session_id = request.POST['session_id']
	session = Session.objects.get(pk=session_id)
	session.end = datetime.now()
	session.save()
	return HttpResponse(" ")
	
def signout(request):
	logout(request)
	return HttpResponseRedirect('/')

def my_sessions(request):
	trainee = Practitioner.objects.get(id=request.user.id)
	sessions = Session.objects.filter(trainee=trainee)
	return render_to_response("my_sessions.html", {'sessions':sessions}, RequestContext(request))

def rounds(request, session_id):
	trainee = Practitioner.objects.get(id=request.user.id)
	sessions = Session.objects.filter(trainee=trainee)
	return render_to_response("my_sessions.html", {'sessions':sessions}, RequestContext(request))
